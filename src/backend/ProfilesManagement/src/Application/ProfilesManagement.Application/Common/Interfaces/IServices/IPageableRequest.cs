namespace ProfilesManagement.Application.Common.Interfaces.IServices;

public interface IPageableRequest
{
    int PageIndex { get; }
    int PageSize { get; }
}