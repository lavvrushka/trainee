# План реализации Stream Monitoring (4 дня)

Документ описывает пошаговый план реализации подсистемы мониторинга live `HLS/DASH` потоков на базе двух исходных документов:

- `docs/stream-monitoring-design-spec.ru.md` — функциональные и архитектурные требования;
- `docs/stream-monitoring-implementation.ru.md` — практическая схема реализации.

План рассчитан на одного backend-разработчика, работающего полный день (~8 часов эффективной работы). Все работы ведутся в текущем решении `backend/` с добавлением нового проекта `Evrideo.ContentPortal.StreamChecking` и расширением `Application`, `Infrastructure`, `Api`.

Принципы плана:

- каждый день завершается рабочим и проверяемым результатом (не "полуфабрикатом");
- библиотека `StreamChecking` строится полностью изолированной от БД, API, DI конкретного приложения;
- интеграция в основное приложение выполняется только на 4-й день;
- тесты пишутся в тот же день, что и покрываемый код, а не откладываются "на потом".

---

## Общий обзор

| День | Фокус | Зона кода |
|------|-------|-----------|
| 1 | Skeleton библиотеки, модели, contracts, manifest loader + HLS/DASH parsing | `StreamChecking` |
| 2 | Обнаружение новых segments, загрузка, временное хранилище, state comparator | `StreamChecking` |
| 3 | Media probe (FFmpeg), video freeze detection, audio silence detection, engine pipeline | `StreamChecking` |
| 4 | Orchestrator в `Application`, persistence в `Infrastructure`, background service, API, alerts | `Application` / `Infrastructure` / `Api` |

Итоговые deliverables к концу 4-го дня:

- независимая библиотека `Evrideo.ContentPortal.StreamChecking` с полным pipeline;
- оркестратор `StreamMonitoringService` в `Application`;
- `FfmpegMediaProbe`, `StreamMonitoringStateStore`, `StreamAlertRepository`, background service в `Infrastructure`;
- REST endpoints в `Api` (manual trigger, status, active alerts);
- рабочие unit + integration тесты;
- конфигурация через `appsettings.json` и per-channel overrides;
- structured logging по всем ключевым этапам;
- режим наблюдения (calibration) без production alerts.

---

## День 1 — Skeleton, модели, contracts, manifest layer

### 1.1 Цель дня

К концу дня существует компилирующийся проект `Evrideo.ContentPortal.StreamChecking` с полностью описанными контрактами, моделями данных, реализованным слоем загрузки и разбора manifest (`HLS` + `DASH`) и unit-тестами на parsers. Engine ещё не собран, но все зависимости manifest-слоя готовы к подключению.

### 1.2 Структура задач

#### 1.2.1 Создание проекта и подключение к solution

- Создать `backend/src/Evrideo.ContentPortal.StreamChecking/Evrideo.ContentPortal.StreamChecking.csproj` как `netstandard2.1`/`net10.0` библиотеку без ссылок на `Domain`, `Application`, `Infrastructure`, `Api`.
- Добавить проект в solution `backend/Evrideo.ContentPortal.sln`.
- Создать `backend/tests/Evrideo.ContentPortal.StreamChecking.Tests/Evrideo.ContentPortal.StreamChecking.Tests.csproj` (xUnit + FluentAssertions + NSubstitute).
- Добавить папки по схеме implementation spec (§3):
  - `Abstractions/`
  - `Models/`
  - `Hls/`
  - `Dash/`
  - `Evaluation/`
  - `Media/`
  - `Transport/`
  - `DependencyInjection.cs` (заглушка).
- Убедиться, что проект проходит `dotnet build` без warnings.

#### 1.2.2 Модели данных (`Models/`)

Согласно implementation spec §6:

- `StreamCheckRequest` — `StreamUrl`, `Settings`, `PreviousState`, `ChannelId` (opaque string, просто идентификатор для логов).
- `StreamCheckSettings` — `ManifestRequestTimeout`, `SegmentRequestTimeout`, `PollingInterval`, `NoNewSegmentsTimeout`, `VideoFreezeTimeout`, `AudioSilenceTimeout`, `SilenceThreshold` (dB), плюс `MaxSegmentsPerCycle`, `RetryCount`, `RetryDelay`, `CalibrationMode` (bool).
- `StreamCheckState` — `LastSegmentIdentity`, `LastNewSegmentObservedAt`, `PreviousVideoKeyframeHash`, `AccumulatedFreezeDuration`, `AccumulatedSilenceDuration`, `LastUpdatedAt`. Класс immutable-like (`with`-clauses / record).
- `StreamCheckResult` — `Findings` (`IReadOnlyList<Finding>`), `UpdatedState`, `Diagnostics`.
- `Finding` — `StreamCheckErrorCode Code`, `string Description`, `FindingSeverity Severity` (`Info`/`Warning`/`Error`), `FindingCategory Category` (`ConfirmedStreamFailure` / `TransientMonitoringFailure` / `PersistentMonitoringFailure`).
- `StreamCheckDiagnostics` — `Protocol`, `StartedAt`, `CompletedAt`, `NewSegmentsCount`, `LastSegmentIdentity`, `VideoFreezeObserved`, `AudioSilenceObserved`, `RawLog` (list of strings для дебага).
- `enum StreamCheckErrorCode` — `ManifestUnavailable`, `ManifestInvalid`, `NoNewSegments`, `SegmentDownloadFailed`, `VideoFrozen`, `AudioSilent`, `ProbeExecutionFailed`.

Все модели — `record`/`record class` с публичным init-only API, без JSON атрибутов (библиотека независима от сериализации).

#### 1.2.3 Контракты (`Abstractions/`)

- `IStreamCheckEngine` — `Task<StreamCheckResult> RunAsync(StreamCheckRequest request, CancellationToken ct)`.
- `IManifestLoader` — `Task<ManifestLoadResult> LoadAsync(Uri url, TimeSpan timeout, CancellationToken ct)`.
- `IHlsParser` — `HlsManifestSnapshot Parse(string body, Uri baseUri)`.
- `IDashParser` — `DashManifestSnapshot Parse(string body, Uri baseUri, DateTimeOffset now)`.
- `IProtocolDetector` — `StreamProtocol Detect(Uri url, string body)`.
- `ISegmentDownloader` — `Task<SegmentDownloadResult> DownloadAsync(SegmentReference reference, TimeSpan timeout, CancellationToken ct)` (реализация — день 2).
- `IMediaProbe` — `Task<VideoProbeResult> ExtractFirstKeyframeAsync(...)` + `Task<AudioProbeResult> AnalyzeAudioAsync(...)` (реализация — день 3).
- `IStateComparer` — `StateComparisonResult Compare(ManifestSnapshot snapshot, StreamCheckState previous, StreamCheckSettings settings, DateTimeOffset now)` (реализация — день 2).

Все контракты полностью синхронно-документированные через XML comments.

#### 1.2.4 Transport слой (`Transport/`)

- `HttpManifestLoader : IManifestLoader` — работает через инжектируемый `HttpClient`. Возвращает `ManifestLoadResult` с полями `Success`, `HttpStatus`, `Body`, `ElapsedMs`, `Error` (отделяется network error от parse error).
- `ManifestLoadResult` содержит только технические поля и не принимает решений "валиден/невалиден" — это делает parser.
- Таймаут реализуется через `CancellationTokenSource.CreateLinkedTokenSource` + `cts.CancelAfter(timeout)`; network error и таймаут различаются.

#### 1.2.5 Protocol detection (`Abstractions/ProtocolDetector.cs`)

Согласно implementation spec §7.1:

- предварительно по URL: `*.m3u8` → `Hls`, `*.mpd` → `Dash`.
- окончательно по содержимому: тело начинается с `#EXTM3U` → `Hls`, содержит `<MPD` или XML namespace `urn:mpeg:dash:schema:mpd:2011` → `Dash`.
- если оба признака конфликтуют — приоритет у контента.
- если определить невозможно → `StreamProtocol.Unknown`, далее pipeline формирует `ManifestInvalid`.

#### 1.2.6 HLS parser (`Hls/HlsParser.cs`)

Без использования внешних парсеров — ручная реализация строкового разбора достаточной глубины для live-мониторинга.

- Распознать master vs media playlist (наличие `#EXT-X-STREAM-INF`).
- Master: извлечь variants `#EXT-X-STREAM-INF` с дочерними playlist URI + renditions `#EXT-X-MEDIA:TYPE=AUDIO`.
- Media playlist: извлечь `#EXT-X-TARGETDURATION`, `#EXT-X-MEDIA-SEQUENCE`, `#EXT-X-DISCONTINUITY-SEQUENCE`, `#EXTINF` + segment URI, `#EXT-X-BYTERANGE`, `#EXT-X-ENDLIST` (если есть — поток помечается VOD, для live это важно).
- Вернуть `HlsManifestSnapshot`: `IsMaster`, `Variants`, `AudioRenditions`, `MediaPlaylist` (для media), `IsVod`.
- Для master parser НЕ загружает дочерние playlists сам — только возвращает их URI. Загрузка делегируется engine на следующий день (и использует тот же `IManifestLoader`).

#### 1.2.7 DASH parser (`Dash/DashParser.cs`)

- Разобрать `MPD` как XML через `System.Xml.Linq` (нет внешних зависимостей).
- Извлечь `@type` (`static`/`dynamic`), `@availabilityStartTime`, `@publishTime`, `@minimumUpdatePeriod`, `@timeShiftBufferDepth`, `@suggestedPresentationDelay`.
- Для каждого `Period` → `AdaptationSet` → `Representation`:
  - `@mimeType`, `@codecs`, `@bandwidth`, `@id`;
  - `SegmentTemplate` с `@media`, `@initialization`, `@timescale`, `@startNumber`, `@duration` или `SegmentTimeline` (`S` элементы с `t`, `d`, `r`).
- Реализовать функцию разрешения ожидаемых segments для текущего live-окна:
  - по номеру (`$Number$`) если используется `@duration`;
  - по времени (`$Time$`) если используется `SegmentTimeline`.
- Вернуть `DashManifestSnapshot`: список `AdaptationSetSnapshot`, каждый с `Representations` и уже вычисленным `CurrentSegments` (последние N, где N ≤ `MaxSegmentsPerCycle`).

#### 1.2.8 Общий `ManifestSnapshot` (`Models/ManifestSnapshot.cs`)

Нормализованное представление независимо от протокола:

- `Protocol`;
- `IsLive`;
- `VideoSegments` — список `SegmentReference` (`Identity`, `Uri`, `Duration`, `MediaType`);
- `AudioSegments` — список по track id;
- `RawDurationSeconds`;
- `FetchedAt`.

`HlsManifestSnapshot` и `DashManifestSnapshot` мапятся в `ManifestSnapshot` через отдельный `ManifestNormalizer`.

#### 1.2.9 Unit tests (1-й день)

В `Evrideo.ContentPortal.StreamChecking.Tests`:

- `HlsParserTests`:
  - master playlist с 2 variants + audio rendition;
  - media playlist с 10 сегментами;
  - media playlist с `#EXT-X-ENDLIST` (VOD) должен помечаться `IsVod=true`;
  - невалидный ввод (нет `#EXTM3U` в начале) — parser должен кидать `ManifestParseException`.
- `DashParserTests`:
  - `MPD` с `SegmentTemplate + @duration`;
  - `MPD` с `SegmentTemplate + SegmentTimeline`;
  - `MPD` с `@type=static` (помечается как не-live).
- `ProtocolDetectorTests`: по url, по body, конфликт.
- `HttpManifestLoaderTests`: моки `HttpMessageHandler`, проверка таймаута, проверка что network error не помечается как parse error.

Тестовые фикстуры (`.m3u8`, `.mpd`) в `tests/Evrideo.ContentPortal.StreamChecking.Tests/Fixtures/`.

### 1.3 Что должно получиться

- `dotnet build` проходит без warnings.
- `dotnet test` прогоняет все тесты 1-го дня и они зелёные (~20–30 тестов).
- В проекте `StreamChecking` нет ни одной ссылки на `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore.*`, `Serilog`, `Hangfire` или любой проект решения.
- Можно вручную написать код, который загружает `https://demo.unified-streaming.com/.../live.isml/.m3u8` и получает список segments (даже без дальнейшего анализа).

### 1.4 Риски и on-the-fly решения

- Живые DASH с `MPD@type=dynamic` + `SegmentTimeline` нестандартно сегментируются у разных вендоров. Если парсер падает на реальном потоке — фиксируется только базовый кейс, edge cases логируются как TODO и переносятся в тесты.
- Отсутствие `InitializationSegment` у DASH — допускается, он просто не скачивается отдельно в дне 2, но URL сохраняется в snapshot.

---

## День 2 — Detection, download, temp storage, state comparison

### 2.1 Цель дня

К концу дня библиотека умеет: получить URL → загрузить manifest → разобрать → построить `ManifestSnapshot` → сравнить с предыдущим `StreamCheckState` → определить новые segments → скачать их во временный каталог → корректно очистить файлы. Engine ещё не собран полностью, но все промежуточные компоненты работают и покрыты тестами.

### 2.2 Структура задач

#### 2.2.1 State comparer (`Evaluation/StateComparer.cs`)

Реализация `IStateComparer` согласно implementation spec §8:

- вход: `ManifestSnapshot snapshot`, `StreamCheckState previous`, `StreamCheckSettings settings`, `DateTimeOffset now`;
- выход: `StateComparisonResult`:
  - `NewVideoSegments : IReadOnlyList<SegmentReference>`;
  - `NewAudioSegmentsByTrack : IReadOnlyDictionary<string, IReadOnlyList<SegmentReference>>`;
  - `NoNewSegmentsDetected : bool`;
  - `CandidateUpdatedState : StreamCheckState` (state без учёта video/audio findings — они добавятся позже).

Логика:

- если `previous.LastSegmentIdentity == null` → это первое успешное наблюдение, `NewVideoSegments` = последние N segments из snapshot (где N — минимум из `MaxSegmentsPerCycle` и длины snapshot), `LastNewSegmentObservedAt = now`, `NoNewSegmentsDetected = false`.
- иначе — найти в snapshot первый segment, `Identity` которого совпадает с `previous.LastSegmentIdentity`, новые segments — всё после него.
- если новых нет:
  - если `now - previous.LastNewSegmentObservedAt > settings.NoNewSegmentsTimeout` → `NoNewSegmentsDetected = true`;
  - иначе — просто `NewVideoSegments` пустой и state сохраняется.
- если segment с `previous.LastSegmentIdentity` не найден (например, он уже ушёл из live-окна) → трактовать все segments snapshot как потенциально новые, но ограничить `MaxSegmentsPerCycle`, и залогировать warning "sliding window overflow" (не ошибка — монитор просто отстал).

#### 2.2.2 Segment downloader (`Transport/SegmentDownloader.cs`)

Реализация `ISegmentDownloader`:

- `Task<SegmentDownloadResult> DownloadAsync(SegmentReference reference, string tempDirectory, TimeSpan timeout, CancellationToken ct)`.
- выполняет `HttpClient.GetAsync` с `HttpCompletionOption.ResponseHeadersRead` + streaming copy в файл внутри `tempDirectory`;
- имя файла: `{channelId}_{guid}_{segmentIndex}{originalExtension}` — детерминировано и без коллизий;
- таймаут через `CancellationTokenSource.CreateLinkedTokenSource`;
- проверяет, что файл не пустой и не меньше, чем `MinValidSegmentBytes` (64, настраиваемо);
- `SegmentDownloadResult` — `Success`, `TempFilePath`, `SizeBytes`, `ElapsedMs`, `HttpStatus`, `ErrorKind` (`NotFound`/`Timeout`/`NetworkError`/`EmptyPayload`/`Unknown`);
- при любой ошибке файл удаляется немедленно; пустой/повреждённый файл — тоже.

#### 2.2.3 Temp storage lifecycle (`Transport/TempSegmentStorage.cs`)

Согласно implementation spec §9.1:

- класс `TempSegmentStorage` принимает `rootDirectory` (из конфига, напр. `C:\temp\content-portal\monitoring` или `/var/tmp/content-portal-monitoring`);
- `CreateCycleScope(string channelId)` возвращает `IDisposable`-scope с гарантированной очисткой директории цикла при `Dispose` (в `finally`, даже если процесс падает);
- каждый check cycle работает в своей subdirectory `{root}/{channelId}/{cycleId}` — чтобы orphan cleanup видел, кому принадлежали файлы;
- метод `CleanupOrphansAsync(TimeSpan maxAge, CancellationToken ct)` удаляет subdirectories старше `maxAge` — этот метод вызовет background service из `Infrastructure` в день 4.

#### 2.2.4 Network retry strategy (`Transport/NetworkRetryPolicy.cs`)

Согласно implementation spec §18.2:

- один общий helper: `Task<T> ExecuteWithRetryAsync<T>(Func<CancellationToken, Task<T>> op, int retryCount, TimeSpan retryDelay, Func<T, bool> isTransient, CancellationToken ct)`.
- retry считается `transient`, если ошибка: `HttpRequestException` (без `StatusCode` 4xx), `TaskCanceledException` от своего CTS (таймаут), `IOException` при чтении ответа.
- 4xx не ретраится (кроме 408, 425, 429).
- используется `ManifestLoader` и `SegmentDownloader`, оба передают результат ретрай-хелперу.

#### 2.2.5 Error classification helper (`Evaluation/FailureClassifier.cs`)

Согласно design spec §9.1 и implementation spec §18.1:

- `FindingCategory ClassifyNetworkFailure(int? attemptsMade, int attemptsAllowed, bool isMonitorSideSuspected)`;
- если все попытки исчерпаны и отказ системный (DNS, connect reset) — `PersistentMonitoringFailure`;
- если одна-две попытки upali но последующая успешна (ветка не будет использоваться, но API готов);
- подтверждённая monitor-side ошибка при ручной интерпретации → `TransientMonitoringFailure`;
- stream content failure (например, `NoNewSegments`) → `ConfirmedStreamFailure`.

Этот классификатор вызывается engine при формировании `Finding`.

#### 2.2.6 Engine — частичная сборка (`Evaluation/StreamCheckEngine.cs`)

Уже можно собрать skeleton engine, который на dне 2 работает до стадии "новые segments скачаны":

1. `IManifestLoader.LoadAsync`;
2. `IProtocolDetector.Detect`;
3. parse HLS/DASH → `ManifestSnapshot`;
4. для HLS master — догрузить child playlists через тот же loader;
5. `IStateComparer.Compare`;
6. `foreach newSegment → ISegmentDownloader.DownloadAsync`;
7. до стадии media analysis доходим, но `IMediaProbe` — stub, возвращающий "not implemented yet" → engine помечает `ProbeExecutionFailed` как `TransientMonitoringFailure` (временно, пока день 3 не закончен).

Это даёт возможность запустить engine end-to-end уже на 2-й день и увидеть на реальном потоке все этапы кроме media.

#### 2.2.7 Unit tests (2-й день)

- `StateComparerTests`:
  - первая проверка (previous state отсутствует) — все last N помечаются как новые;
  - последующая проверка, последний сегмент совпадает — новых нет;
  - последующая проверка, появились 2 новых segments;
  - sliding window overflow (previous.LastSegmentIdentity ушёл из окна);
  - NoNewSegments через >timeout;
  - NoNewSegments НЕ формируется, если прошло меньше timeout.
- `SegmentDownloaderTests`:
  - успешная загрузка → файл существует, размер > 0;
  - 404 → файл удалён, `ErrorKind = NotFound`;
  - таймаут → `ErrorKind = Timeout`, файл удалён;
  - пустой response → `ErrorKind = EmptyPayload`, файл удалён.
- `TempSegmentStorageTests`:
  - scope создаёт subdirectory; `Dispose` её удаляет вместе с содержимым;
  - даже при исключении внутри scope директория удаляется.
  - `CleanupOrphansAsync` удаляет только subdirectories старше `maxAge`.
- `NetworkRetryPolicyTests`:
  - transient сбой → retry → success;
  - non-transient 404 → сразу fail без retry.

### 2.3 Что должно получиться

- engine с мокированным `IMediaProbe` на реальном публичном HLS-потоке (например, Apple sample stream) запускается и корректно проходит до "downloaded N new segments";
- временные файлы гарантированно удаляются после каждого цикла;
- `NoNewSegments` корректно формируется, если указать искусственно маленький `NoNewSegmentsTimeout`;
- `dotnet test` — ~50–60 тестов, все зелёные;
- ещё нет ни одного упоминания БД, DI контейнера основного приложения или `Hosted Service` внутри проекта `StreamChecking`.

### 2.4 Риски и on-the-fly решения

- некоторые публичные HLS потоки отдают segments через CDN с redirect — `HttpClient` должен `AllowAutoRedirect = true` (по умолчанию уже так). Протестировать отдельно.
- некоторые DASH используют `BaseURL` elements — если в первый день это не было учтено, на день 2 DASH normalizer обязательно должен применять `BaseURL` перед построением snapshot.

---

## День 3 — Media probe (FFmpeg), video freeze, audio silence, engine finalization

### 3.1 Цель дня

К концу дня библиотека целиком функциональна: видео freeze и audio silence детектируются по реальным файлам, engine собирает все findings, обновляет `StreamCheckState`, возвращает `StreamCheckResult`. Запуск на реальном HLS потоке возвращает осмысленные результаты (без падений).

### 3.2 Структура задач

#### 3.2.1 `IMediaProbe` (финальные контракты, `Abstractions/IMediaProbe.cs`)

```
Task<VideoProbeResult> ExtractFirstKeyframeAsync(
    string segmentFilePath,
    string outputImagePath,
    TimeSpan timeout,
    CancellationToken ct);

Task<AudioProbeResult> AnalyzeAudioAsync(
    string segmentFilePath,
    TimeSpan timeout,
    CancellationToken ct);
```

`VideoProbeResult`:
- `Success : bool`;
- `FrameHash : ulong` (pHash);
- `FrameImagePath : string?`;
- `ErrorKind` (`NoKeyframe`/`DecodeError`/`ProcessFailure`/`Timeout`).

`AudioProbeResult`:
- `Success : bool`;
- `Tracks : IReadOnlyList<AudioTrackResult>` с `TrackIndex`, `MaxVolumeDb`, `MeanVolumeDb`;
- `ErrorKind`.

#### 3.2.2 `FfmpegMediaProbe` (`Media/FfmpegMediaProbe.cs`)

⚠ Класс находится в проекте `StreamChecking`, но сам `FFmpeg` (binary) должен быть доступен через конфигурацию `FfmpegOptions.BinaryPath`. Библиотека не зависит от IOptions — принимает строку через конструктор.

- Спавнит `ffmpeg` процесс через `System.Diagnostics.Process` с `CreateNoWindow = true`, `RedirectStandardError = true`, `RedirectStandardOutput = true`.
- Для video: команда
  `ffmpeg -hide_banner -nostats -i <segment> -frames:v 1 -vf "select=eq(pict_type\,I),scale=64:64" -f image2 <output.png>`
  → первый keyframe, масштабированный до 64×64 (для стабильного hash).
- Для audio: команда
  `ffmpeg -hide_banner -nostats -i <segment> -map 0:a -af volumedetect -f null -`
  → парсим из stderr строки `max_volume: -XX.X dB` и `mean_volume`, по одной на track.
- Таймаут через `Process.WaitForExitAsync(ct)` + отдельный `CancellationTokenSource.CancelAfter` + `Process.Kill(entireProcessTree: true)` при таймауте.
- Коды возврата ≠ 0 конвертируются в `ErrorKind = ProcessFailure`.
- stderr всегда буферизуется в ring-buffer последних 64КБ и попадает в `Diagnostics.RawLog` для отладки.

#### 3.2.3 Perceptual hash (`Media/PerceptualHash.cs`)

- Читает PNG 64×64 через `System.Drawing.Common` (на Windows) либо `SixLabors.ImageSharp` (кросс-платформенно — предпочтительно).
- Вычисляет average hash: приведение к grayscale, mean brightness, каждый пиксель → 0 или 1 → 64-bit ulong.
- Сравнение через Hamming distance; threshold — 6 бит по умолчанию (настраивается).

ImageSharp добавляется как единственная non-trivial зависимость в `StreamChecking.csproj`. Обосновано, т.к. альтернатива (System.Drawing.Common) не работает на Linux в .NET 10 без дополнительных native deps.

#### 3.2.4 Video freeze evaluator (`Evaluation/VideoFreezeEvaluator.cs`)

Реализует алгоритм из design spec §7.5 / implementation spec §10.4:

- вход: список `VideoProbeResult` для новых segments, `previous.PreviousVideoKeyframeHash`, `previous.AccumulatedFreezeDuration`, длительность segment (из snapshot) или fallback `PollingInterval`, `settings.VideoFreezeTimeout`;
- для каждого probed segment:
  - если probe `!Success` → пропускаем этот segment, НЕ обнуляем accumulated duration (это технический сбой, не stream issue) → возможно `Finding(ProbeExecutionFailed, TransientMonitoringFailure)` — но только один раз за цикл;
  - если predecessor hash отсутствует → просто запоминаем;
  - если `HammingDistance(current, previous) <= threshold` → `accumulated += segmentDuration`;
  - иначе → `accumulated = 0`;
  - обновляем `previousHash = current`.
- если итоговый `accumulated > VideoFreezeTimeout` → `Finding(VideoFrozen, Error, ConfirmedStreamFailure)`.

#### 3.2.5 Audio silence evaluator (`Evaluation/AudioSilenceEvaluator.cs`)

Реализует design spec §7.6 / implementation spec §11.3:

- вход: `AudioProbeResult` для каждого нового segment (по всем track), `previous.AccumulatedSilenceDuration`, `settings.SilenceThreshold`, `settings.AudioSilenceTimeout`;
- для каждого segment:
  - берём все tracks. Если `MaxVolumeDb` у ВСЕХ `< SilenceThreshold` → segment считается silent → `accumulated += segmentDuration`;
  - если хотя бы на одной дорожке сигнал ≥ threshold → `accumulated = 0`.
- если `accumulated > AudioSilenceTimeout` → `Finding(AudioSilent, Error, ConfirmedStreamFailure)`.

#### 3.2.6 Calibration mode

Согласно design spec §7.7 и implementation spec §11.4:

- если `Settings.CalibrationMode = true`, findings `VideoFrozen` и `AudioSilent` формируются с `Severity = Info` и `Category = ConfirmedStreamFailure`, но engine отмечает в `Diagnostics.RawLog` специальную строку "CALIBRATION: would trigger X".
- downstream (Application layer) в день 4 не поднимет production alerts для Info-findings.

Это ключевая защита от ложных срабатываний при первом включении на реальных каналах.

#### 3.2.7 Финальная сборка `StreamCheckEngine`

Полный pipeline согласно design spec §13:

1. fetch manifest (с retry) → если fail → `Finding(ManifestUnavailable)` классифицированный как monitor-side vs stream-side по результатам retry → return;
2. detect protocol → если `Unknown` → `Finding(ManifestInvalid, ConfirmedStreamFailure)` → return;
3. parse → если parse error → `Finding(ManifestInvalid, ConfirmedStreamFailure)` → return;
4. для HLS master — загрузить все child playlists, обработать ошибки;
5. normalize → `ManifestSnapshot`;
6. `IStateComparer.Compare` → получить new segments;
7. если `NoNewSegmentsDetected` → `Finding(NoNewSegments, ConfirmedStreamFailure)`;
8. создать temp scope: `using var scope = _tempStorage.CreateCycleScope(request.ChannelId);`;
9. для каждого new video segment — download → probe video keyframe;
10. для каждого new audio segment (по trackId) — download → probe audio (если video и audio идут в одном segment, повторная загрузка не выполняется — используется один и тот же file path, проверяется через `SegmentReference.Uri` set);
11. evaluate video freeze;
12. evaluate audio silence;
13. построить итоговый `StreamCheckState` — объединить `CandidateUpdatedState` из comparer с новыми `AccumulatedFreezeDuration`, `AccumulatedSilenceDuration`, `PreviousVideoKeyframeHash`;
14. вернуть `StreamCheckResult`.

В `finally` (scope.Dispose) — гарантированная очистка всех temp files цикла.

#### 3.2.8 DI helper (`DependencyInjection.cs`)

- `public static IServiceCollection AddStreamChecking(this IServiceCollection services, Action<StreamCheckingOptions> configure)`.
- регистрирует `HttpClient` через `IHttpClientFactory` с правильным `HttpClientHandler` (pooled, keep-alive).
- регистрирует все internal контракты как `transient` (engine, comparer, evaluators, normalizer, parsers, detector), кроме `TempSegmentStorage` (singleton) и `FfmpegMediaProbe` (singleton).

#### 3.2.9 Unit tests (3-й день)

- `VideoFreezeEvaluatorTests`:
  - 5 подряд одинаковых hash, timeout = 4 сек, segment duration = 1 сек → `VideoFrozen`;
  - 3 одинаковых + 1 отличающийся → accumulator сбрасывается, без finding;
  - probe failure не сбрасывает accumulator.
- `AudioSilenceEvaluatorTests`:
  - все tracks silent → finding;
  - одна track не silent → нет finding;
  - отсутствие audio tracks → `ProbeExecutionFailed`, без audio finding.
- `StreamCheckEngineTests` (с моками):
  - happy path: manifest → segments → нет findings;
  - manifest 500 → `ManifestUnavailable`;
  - parse fail → `ManifestInvalid`;
  - все probe-моки возвращают одинаковый hash долго → `VideoFrozen`;
  - probe audio возвращает -60 dB для всех tracks → `AudioSilent`.

#### 3.2.10 Integration test (`Tests.Integration`)

Отдельный проект `Evrideo.ContentPortal.StreamChecking.IntegrationTests` (запускается только локально через trait `[Trait("Category", "Integration")]`, не в CI по умолчанию):

- реальный FFmpeg (путь из env var `FFMPEG_BINARY_PATH`);
- реальный публичный live HLS поток (Apple Advanced sample stream);
- 3 последовательных цикла с паузой `PollingInterval` между ними;
- проверка: `ManifestSnapshot` не пуст, новые segments обнаружены, video probe возвращает hash, audio probe возвращает уровень громкости, freeze не срабатывает на нормальном потоке.

### 3.3 Что должно получиться

- библиотека полностью функциональна и самодостаточна;
- на реальном HLS потоке integration test проходит;
- на искусственно "замороженном" тестовом mp4 (статичная картинка + тишина) engine корректно формирует `VideoFrozen` и `AudioSilent`;
- ~90–110 unit-тестов, все зелёные;
- FFmpeg используется строго через `IMediaProbe`, никто другой о нём не знает.

### 3.4 Риски и on-the-fly решения

- некоторые H.264 потоки в HLS имеют длинные GOP (2+ сек), и "первый keyframe" в каждом segment может совпадать между соседними segments даже при нормальном потоке (edit decision). Если на калибровке это даёт ложные срабатывания — в день 3 этот кейс оставляется за `CalibrationMode`, а в alert-правила добавится "требуется N последовательных подтверждений" в день 4.
- FFmpeg на Windows может быть не в PATH; путь обязательно конфигурируется явно через options.
- для ULL (low-latency) HLS (LL-HLS с `#EXT-X-PART`) parser дня 1 может некорректно трактовать partial segments. На день 3 это лечится lazy: partial segments игнорируются (только complete) + TODO на будущее.

---

## День 4 — Integration: orchestrator, persistence, background service, API

### 4.1 Цель дня

К концу дня подсистема полностью интегрирована в `ContentPortal`: БД хранит state и alerts, фоновая служба периодически проверяет каналы с включённым мониторингом, API умеет запускать ручную проверку и читать активные alerts. Всё собирается, запускается, логируется.

### 4.2 Структура задач

#### 4.2.1 Domain модели (`Evrideo.ContentPortal.Domain`)

- `StreamMonitoringConfiguration` (per-channel):
  - `ChannelId : Guid`;
  - `IsMonitoringEnabled : bool`;
  - `StreamUrl : string`;
  - `PollingIntervalOverride : TimeSpan?`, и аналогичные override для всех параметров из `StreamCheckSettings` (nullable);
  - `CalibrationMode : bool`.
- `StreamMonitoringSnapshot` (persistent state):
  - `ChannelId`;
  - `LastSegmentIdentity`;
  - `LastNewSegmentObservedAt`;
  - `PreviousVideoKeyframeHash`;
  - `AccumulatedFreezeDuration`;
  - `AccumulatedSilenceDuration`;
  - `LastUpdatedAt`.
- `StreamAlert`:
  - `Id : Guid`;
  - `ChannelId`;
  - `Code : string` (из `StreamCheckErrorCode`);
  - `Category` (из `FindingCategory`);
  - `Status : AlertStatus` (`Open`/`Closed`);
  - `OpenedAt : DateTimeOffset`;
  - `ClosedAt : DateTimeOffset?`;
  - `LastSeenAt : DateTimeOffset`;
  - `Description : string`.

#### 4.2.2 Infrastructure: persistence (`Evrideo.ContentPortal.Infrastructure/Monitoring/`)

- EF Core entity configurations для трёх entities: `StreamMonitoringConfigurationConfiguration`, `StreamMonitoringSnapshotConfiguration`, `StreamAlertConfiguration`.
- `StreamMonitoringStateStore : IStreamMonitoringStateStore` (контракт в `Application`) с методами:
  - `Task<StreamCheckState?> LoadAsync(Guid channelId, CancellationToken ct)`;
  - `Task SaveAsync(Guid channelId, StreamCheckState state, CancellationToken ct)`;
  - маппинг между domain `StreamMonitoringSnapshot` и library `StreamCheckState`.
- `StreamAlertRepository : IStreamAlertRepository` с методами:
  - `Task<IReadOnlyList<StreamAlert>> GetActiveAsync(Guid channelId, CancellationToken ct)`;
  - `Task ApplyAsync(Guid channelId, IReadOnlyList<Finding> findings, DateTimeOffset now, CancellationToken ct)` — логика alert reconciliation (§4.2.5).
- EF Core migration `AddStreamMonitoring` — три новые таблицы.

#### 4.2.3 Infrastructure: FFmpeg options (`Monitoring/Media/`)

- `FfmpegOptions` (POCO, bound from `appsettings.json` section `Monitoring:Ffmpeg`): `BinaryPath`, `WorkingDirectory`.
- регистрация `IMediaProbe` через delegate, читающий options.

#### 4.2.4 Application: `StreamMonitoringService` (`Application/UseCases/StreamMonitoring/`)

- `IStreamMonitoringService` + реализация:
  - `Task<StreamCheckResult> RunOnceAsync(Guid channelId, CancellationToken ct)`;
  - `Task<StreamMonitoringStatusDto> GetStatusAsync(Guid channelId, CancellationToken ct)`;
  - `Task<IReadOnlyList<StreamAlertDto>> GetActiveAlertsAsync(Guid channelId, CancellationToken ct)`.
- `RunOnceAsync` алгоритм:
  1. загрузить `StreamMonitoringConfiguration` по `channelId`;
  2. смёрджить default settings (из `IOptions<StreamCheckingDefaults>`) с overrides;
  3. загрузить `StreamCheckState` через `IStreamMonitoringStateStore`;
  4. построить `StreamCheckRequest`;
  5. вызвать `IStreamCheckEngine.RunAsync`;
  6. сохранить `UpdatedState`;
  7. `IStreamAlertRepository.ApplyAsync` — reconcile findings;
  8. залогировать `Diagnostics` структурированно (serilog / MS logger);
  9. вернуть `StreamCheckResult`.
- DTO (`StreamMonitoringStatusDto`, `StreamAlertDto`, `RunCheckResponseDto`) — plain records без domain moels.

#### 4.2.5 Alert reconciliation (`Infrastructure/Monitoring/Repositories/StreamAlertRepository.cs`)

Согласно implementation spec §15:

- получить текущие open alerts по каналу;
- для каждого `finding` в `findings`:
  - если существует open alert с тем же `Code` → обновить `LastSeenAt = now`;
  - иначе → создать новый open alert (НО только если `Severity != Info`, т.е. не calibration);
- для каждого open alert, которого НЕТ в текущих findings:
  - если `now - LastSeenAt > ClosingGracePeriod` (конфигурируется, default 2× `PollingInterval`) → закрыть alert (`Status = Closed`, `ClosedAt = now`);
  - иначе оставить open (нежный grace, чтобы временное восстановление не закрывало alert преждевременно);
- для ошибок с `Category = TransientMonitoringFailure` или `PersistentMonitoringFailure` alert НЕ создаётся в stream-alerts таблице — они только логируются (можно добавить отдельную таблицу `MonitoringServiceIncident`, но в день 4 ограничиваемся логом).

#### 4.2.6 Background service (`Infrastructure/Monitoring/Background/StreamMonitoringHostedService.cs`)

Согласно implementation spec §14:

- наследует `BackgroundService`;
- каждую итерацию:
  1. получить список `StreamMonitoringConfiguration` с `IsMonitoringEnabled = true`;
  2. для каждого канала запустить проверку через `SemaphoreSlim` (ограничение параллелизма, default 4);
  3. не допустить двойного запуска того же канала через `ConcurrentDictionary<Guid, Task>`;
  4. await всех с continuation, логирующей результат;
  5. ждать `PollingInterval`.
- отдельная задача внутри service — периодическая `CleanupOrphansAsync` для temp directory (раз в 10 минут).
- корректная обработка `stoppingToken` (передать во все downstream calls).

#### 4.2.7 API (`Evrideo.ContentPortal.Api/Controllers/StreamMonitoringController.cs`)

Endpoints:

- `POST /api/v1/channels/{channelId}/stream-monitoring/run` → ручной запуск одной проверки → 202 Accepted + результат;
- `GET /api/v1/channels/{channelId}/stream-monitoring/status` → последний `StreamMonitoringSnapshot` + диагностика последнего прогона (храним в памяти service + last cycle in Redis/memory cache, опционально — в `LastDiagnostics` колонке `StreamMonitoringSnapshot`);
- `GET /api/v1/channels/{channelId}/stream-monitoring/alerts?status=open` → список alerts;
- `GET /api/v1/stream-monitoring/alerts?status=open` → все открытые alerts (для dashboard).

Авторизация через уже существующий в проекте механизм (JWT). Swagger-документирование через XML comments.

#### 4.2.8 Configuration (`Evrideo.ContentPortal.Api/appsettings.json`)

```json
"Monitoring": {
  "Ffmpeg": {
    "BinaryPath": "ffmpeg",
    "WorkingDirectory": null
  },
  "Defaults": {
    "ManifestRequestTimeout": "00:00:10",
    "SegmentRequestTimeout": "00:00:15",
    "PollingInterval": "00:00:30",
    "NoNewSegmentsTimeout": "00:02:00",
    "VideoFreezeTimeout": "00:00:30",
    "AudioSilenceTimeout": "00:00:30",
    "SilenceThreshold": -50.0,
    "MaxSegmentsPerCycle": 3,
    "RetryCount": 2,
    "RetryDelay": "00:00:02",
    "CalibrationMode": true
  },
  "TempStorage": {
    "RootDirectory": "./monitoring-temp",
    "OrphanMaxAge": "00:30:00"
  },
  "Concurrency": {
    "MaxParallelChannels": 4
  },
  "AlertClosingGracePeriod": "00:01:00"
}
```

**`CalibrationMode: true` по умолчанию** — чтобы при первом запуске никто не получил ложные алерты.

#### 4.2.9 DI wiring

- `Evrideo.ContentPortal.Infrastructure.DependencyInjection.AddStreamMonitoring(this IServiceCollection, IConfiguration)`:
  - регистрирует EF entity configs (в том же `DbContext`);
  - регистрирует `IStreamMonitoringStateStore`, `IStreamAlertRepository`, `IMediaProbe` (→ `FfmpegMediaProbe`), `ITempSegmentStorage`;
  - вызывает `services.AddStreamChecking(options => ...)`;
  - регистрирует `StreamMonitoringHostedService`.
- `Evrideo.ContentPortal.Application.DependencyInjection.AddStreamMonitoringApplication`:
  - регистрирует `IStreamMonitoringService`.
- обе секции вызываются из `Program.cs`.

#### 4.2.10 Logging

Structured logging согласно implementation spec §17 через `ILogger<T>` с scope `{ChannelId}`:

- `LogInformation("Stream check started {Url} protocol={Protocol}", ...)`;
- `LogInformation("Manifest loaded in {ElapsedMs}ms status={HttpStatus}", ...)`;
- `LogInformation("New segments detected: {Count}", ...)`;
- `LogWarning("Finding: {Code} category={Category} description={Description}")`;
- `LogInformation("Cycle completed: {DurationMs}ms findings={Findings}")`.

Diagnostics также кратко пишутся в отдельный Serilog sink (или отдельную таблицу `MonitoringCycleLog`, если будет желание — опционально в день 4).

#### 4.2.11 Integration tests (`Evrideo.ContentPortal.IntegrationTests`)

- `WebApplicationFactory`-based тесты:
  - POST run → 202, state сохраняется в БД;
  - GET status → возвращает state;
  - GET alerts → возвращает alerts;
  - повторный run без изменений → alert не дублируется;
  - alert автоматически закрывается после `ClosingGracePeriod` (через mock of `IClock`).
- Не используем реальный FFmpeg/HTTP — через моки `IStreamCheckEngine`.

#### 4.2.12 Migrations + smoke

- применить миграцию `dotnet ef database update` на dev-БД;
- вручную:
  - создать тестовую `StreamMonitoringConfiguration` с реальным публичным HLS;
  - запустить API, проверить: hosted service стартует, первый cycle логируется, alerts таблица ведёт себя корректно;
  - переключить `CalibrationMode=false` и смоделировать freeze (локальный mp4 + mock stream) → появляется alert; убрать источник — alert закрывается.

### 4.3 Что должно получиться

- успешная миграция БД в dev окружении;
- приложение стартует, hosted service в логах периодически пишет "cycle completed";
- API endpoints возвращают ожидаемые ответы;
- рабочее окружение: `FFmpeg` настроен через конфиг, temp directory чистится автоматически;
- calibration mode включён по умолчанию — первый день в production только наблюдение;
- код проходит все существующие CI-проверки (build, test, lint);
- все тесты зелёные: unit (~110) + integration library (~5) + integration api (~10).

### 4.4 Риски и on-the-fly решения

- EF миграция может конфликтовать с существующими миграциями проекта — перед созданием `dotnet ef migrations add` нужно убедиться, что `dotnet ef migrations list` возвращает чистое состояние и что pending changes для других entities отсутствуют.
- `FFmpeg` в docker-образе production — обязательно проверить, есть ли он в базовом образе; если нет — дополнить `Dockerfile` `apt-get install -y ffmpeg` или использовать образ `jrottenberg/ffmpeg` как base. Эта задача может потребовать дополнительного времени вне 4-го дня.
- доступ к `System.Drawing.Common` / `SixLabors.ImageSharp` в published Linux контейнере должен быть проверен в день 4, смоук integration test на реальном потоке подтверждает это.
- формат хранения `StreamCheckState.PreviousVideoKeyframeHash` — `ulong` сериализуется как `long` в PostgreSQL (bigint) через явный cast в `ValueConverter`.

---

## Приложение A — Чеклист по каждому дню

### День 1 — checkpoint

- [ ] Проект `StreamChecking` собирается.
- [ ] Все контракты и модели описаны.
- [ ] Parser HLS покрывает master + media playlist + `ENDLIST`.
- [ ] Parser DASH покрывает `SegmentTemplate + @duration` и `SegmentTimeline`.
- [ ] `HttpManifestLoader` различает network и parse errors.
- [ ] ~20–30 unit-тестов зелёные.

### День 2 — checkpoint

- [ ] `StateComparer` корректно обрабатывает первую проверку, sliding window и NoNewSegments.
- [ ] `SegmentDownloader` сохраняет файлы в temp и удаляет их на ошибках.
- [ ] `TempSegmentStorage` уничтожает cycle scope при `Dispose` и поддерживает orphan cleanup.
- [ ] Engine проходит manifest → download end-to-end на реальном публичном HLS потоке (с mocked `IMediaProbe`).
- [ ] ~50–60 тестов зелёные.

### День 3 — checkpoint

- [ ] `FfmpegMediaProbe` извлекает keyframe + измеряет audio level из реального mp4.
- [ ] Perceptual hash стабилен для одного кадра, различен для разных.
- [ ] `VideoFreezeEvaluator` и `AudioSilenceEvaluator` проходят unit-тесты во всех сценариях.
- [ ] Engine полностью собран; integration test на реальном потоке зелёный.
- [ ] `CalibrationMode` понижает severity findings до Info.
- [ ] ~90–110 тестов зелёные.

### День 4 — checkpoint

- [ ] EF миграция применяется на dev-БД.
- [ ] `StreamMonitoringService` запускает проверку end-to-end.
- [ ] `StreamAlertRepository` корректно open/update/close alerts.
- [ ] Background service запускается, работает под ограничением параллелизма.
- [ ] API endpoints возвращают корректные ответы.
- [ ] `FFmpeg` доступен и настроен через `appsettings.json`.
- [ ] Все тесты зелёные (unit + integration).
- [ ] Первый cycle на реальном канале успешно залогирован.

---

## Приложение B — Вне scope 4 дней

Эти задачи осознанно отложены и должны быть выполнены отдельно:

- UI в frontend (Angular) для просмотра alerts и status;
- notifications (email/Teams/Telegram) по открытию stream-alerts;
- метрики Prometheus/Grafana для cycle duration, findings count;
- reports / historical analytics по alerts;
- LL-HLS partial segments;
- DRM-защищённые потоки;
- многорегиональный мониторинг (несколько monitor nodes с quorum-правилом подтверждения);
- окончательная калибровка `VideoFreezeTimeout` / `SilenceThreshold` на реальных каналах (design spec §7.7 явно требует отдельного согласования).
