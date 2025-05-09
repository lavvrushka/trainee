using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Mongo2Go;
using MongoDB.Driver;
using OfficesManagement.BuisnessLogic.DTOs.Requests;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.DTOs.Requests;
using Xunit;

namespace OfficesManagement.IntegrationTests;

public class OfficeEndpointsTests : IAsyncLifetime
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OfficeEndpointsTests()
    {
        // Запускаем временную MongoDB
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);

        // Создаём WebApplicationFactory, подменяя Mongo-конфиг
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((ctx, conf) =>
                {
                    conf.Sources.Clear();
                    conf.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

                    var overrides = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["MongoSettings:ConnectionString"] = _mongoRunner.ConnectionString,
                            ["MongoSettings:DatabaseName"] = "TestDb"
                        })
                        .Build();
                    conf.AddConfiguration(overrides);
                });
            });

        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Чистим коллекцию Offices перед каждым тестом
        var client = new MongoClient(_mongoRunner.ConnectionString);
        var db = client.GetDatabase("TestDb");
        await db.DropCollectionAsync("Offices");
    }

    [Fact(DisplayName = "POST /api/offices -> 201 Created; then GET /api/offices returns created item")]
    public async Task CreateAndGetAll_ReturnsCreatedOffice()
    {
        // Arrange: используем уникальное имя, чтобы наверняка отличить
        var uniqueName = "Integration Office " + Guid.NewGuid();
        var createReq = new CreateOfficeRequest(
            Name: uniqueName,
            Location: new LocationRequest(
                Address: "123 Test St",
                City: "TestCity",
                State: "TestState",
                Country: "TestCountry"
            ),
            IsActive: true,
            RegistryPhoneNumber: "+100200300"
        );

        // Act #1: POST
        var postResponse = await _client.PostAsJsonAsync("/api/offices", createReq);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act #2: GET all
        var getResponse = await _client.GetAsync("/api/offices");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var stream = await getResponse.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        // Десериализуем только items
        var itemsElement = doc.RootElement.GetProperty("items");
        var offices = itemsElement.Deserialize<OfficeDto[]>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        offices.Should().NotBeNull();
        // Ищем наш именно один уникальный офис
        offices!
            .Where(o => o.Name == uniqueName)
            .Should().ContainSingle(o =>
                o.location.City == "TestCity" &&
                o.RegistryPhoneNumber == "+100200300"
            );
    }

    [Fact(DisplayName = "GET /api/offices/{id} -> 404 for unknown id")]
    public async Task GetById_ReturnsNotFound_ForUnknownId()
    {
        var response = await _client.GetAsync($"/api/offices/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        _mongoRunner.Dispose();
        return Task.CompletedTask;
    }
}
