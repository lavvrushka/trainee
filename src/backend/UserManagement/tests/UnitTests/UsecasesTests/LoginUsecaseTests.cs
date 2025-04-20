using Microsoft.AspNetCore.Identity;
using Moq;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.UseCases.Auth;
using UserManagement.Application.Common.Exeptions;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.Tests.UseCases.Auth
{
    public class LoginHandlerTests
    {
        private readonly Mock<UserManager<Account>> _userManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            var store = new Mock<IUserStore<Account>>();
            _userManagerMock = new Mock<UserManager<Account>>(
                store.Object, null, null, null, null, null, null, null, null);
            _tokenServiceMock = new Mock<ITokenService>();

            _handler = new LoginHandler(
                _userManagerMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsInvalidCredentialsException()
        {
            var request = new UserLoginRequest("test@example.com", "password");
            _userManagerMock
                .Setup(x => x.FindByNameAsync(request.Email))
                .ReturnsAsync((Account)null);

            await Assert.ThrowsAsync<InvalidCredentialsException>(
                () => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsInvalidCredentialsException()
        {
            var request = new UserLoginRequest("test@example.com", "wrongpass");
            var user = new Account();
            _userManagerMock
                .Setup(x => x.FindByNameAsync(request.Email))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidCredentialsException>(
                () => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsUserAuthResponse()
        {
            var request = new UserLoginRequest("test@example.com", "correctpass");
            var user = new Account();
            _userManagerMock
                .Setup(x => x.FindByNameAsync(request.Email))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);

            var tokenPair = new TokensPair
            {
                AccessToken = "access_token_value",
                RefreshToken = "refresh_token_value",
            };
            _tokenServiceMock
                .Setup(x => x.GenerateTokensPairAsync(user))
                .ReturnsAsync(tokenPair);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(tokenPair.AccessToken, result.AccessToken);
            Assert.Equal(tokenPair.RefreshToken, result.RefreshToken);
            _tokenServiceMock.Verify(x => x.GenerateTokensPairAsync(user), Times.Once);
        }
    }
}