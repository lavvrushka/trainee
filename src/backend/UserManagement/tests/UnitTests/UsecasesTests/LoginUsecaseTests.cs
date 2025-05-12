using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserManagement.Application.Common.Exeptions;
using UserManagement.Application.UseCases.Auth;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;
using Xunit;

namespace UserManagement.Application.Tests.UseCases.Auth;

public class LoginHandlerFixture
{
    public Mock<UserManager<Account>> UserManagerMock { get; }
    public Mock<ITokenService> TokenServiceMock { get; }
    public LoginHandler Handler { get; }

    public LoginHandlerFixture()
    {
        var store = new Mock<IUserStore<Account>>();
        UserManagerMock = new Mock<UserManager<Account>>(
            store.Object, null, null, null, null, null, null, null, null);

        TokenServiceMock = new Mock<ITokenService>();

        Handler = new LoginHandler(UserManagerMock.Object, TokenServiceMock.Object);
    }

    public void ResetMocks()
    {
        UserManagerMock.Invocations.Clear();
        UserManagerMock.Reset();
        TokenServiceMock.Invocations.Clear();
        TokenServiceMock.Reset();
    }
}

public class LoginHandlerTests : IClassFixture<LoginHandlerFixture>
{
    private readonly LoginHandlerFixture _fixture;

    public LoginHandlerTests(LoginHandlerFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }

    [Fact(DisplayName = "Login → если пользователь не найден, выбрасывается InvalidCredentialsException")]
    public async Task Handle_UserNotFound_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var request = new UserLoginRequest("test@example.com", "password");

        _fixture.UserManagerMock
            .Setup(x => x.FindByNameAsync(request.Email))
            .ReturnsAsync((Account)null);

        // Act
        var act = async () => await _fixture.Handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage("Неверные учетные данные.");
    }

    [Fact(DisplayName = "Login → если пароль неверный, выбрасывается InvalidCredentialsException")]
    public async Task Handle_InvalidPassword_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var request = new UserLoginRequest("test@example.com", "wrongpass");
        var user = new Account();

        _fixture.UserManagerMock
            .Setup(x => x.FindByNameAsync(request.Email))
            .ReturnsAsync(user);

        _fixture.UserManagerMock
            .Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _fixture.Handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage("Неверные учетные данные.");
    }

    [Fact(DisplayName = "Login → при корректных данных возвращаются токены")]
    public async Task Handle_ValidCredentials_ReturnsTokensPair()
    {
        // Arrange
        var request = new UserLoginRequest("test@example.com", "correctpass");
        var user = new Account();

        var tokens = new TokensPair
        {
            AccessToken = "access_token_value",
            RefreshToken = "refresh_token_value"
        };

        _fixture.UserManagerMock
            .Setup(x => x.FindByNameAsync(request.Email))
            .ReturnsAsync(user);

        _fixture.UserManagerMock
            .Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        _fixture.TokenServiceMock
            .Setup(x => x.GenerateTokensPairAsync(user))
            .ReturnsAsync(tokens);

        // Act
        var result = await _fixture.Handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(tokens.AccessToken);
        result.RefreshToken.Should().Be(tokens.RefreshToken);
        _fixture.TokenServiceMock.Verify(x => x.GenerateTokensPairAsync(user), Times.Once);
    }
}
