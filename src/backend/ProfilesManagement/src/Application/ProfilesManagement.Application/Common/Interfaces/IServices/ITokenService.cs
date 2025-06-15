namespace ProfilesManagement.Application.Common.Interfaces.IServices;

public interface ITokenService
{
    public Guid GetUserIdFromAccessToken();
}
