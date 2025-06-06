using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.DTOs;

public class PaginationDto<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<T> Items { get; set; } = new();
}
public class PageSettingsDto
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
public interface IPageableRequest
{
    int PageIndex { get; }
    int PageSize { get; }
}

public static class PageSettingsMapper
{
    public static PageSettings MapToPageSettings(this IPageableRequest request)
    {
        return new PageSettings
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public static PageSettingsDto MapToPageSettingsDto(this PageSettings settings)
    {
        return new PageSettingsDto
        {
            PageIndex = settings.PageIndex,
            PageSize = settings.PageSize
        };
    }
}

public static class PaginationMapper
{
    public static PaginationDto<TDto> MapToDto<TSource, TDto>(this Pagination<TSource> pagination, Func<TSource, TDto> mapItem)
    {
        if (pagination == null)
        {
            throw new ArgumentNullException(nameof(pagination));
        }

        if (mapItem == null)
        {
            throw new ArgumentNullException(nameof(mapItem));
        }

        return new PaginationDto<TDto>
        {
            CurrentPage = pagination.CurrentPage,
            TotalPages = pagination.TotalPages,
            PageSize = pagination.PageSize,
            TotalCount = pagination.TotalCount,
            Items = pagination.Items.Select(mapItem).ToList()
        };
    }
}
