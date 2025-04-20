using MediatR;
using Moq;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.UseCases.Auth;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.Tests.UseCases.Auth
{
    public class LogoutHandlerTests
    {
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly LogoutHandler _handler;

        public LogoutHandlerTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new LogoutHandler(_tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_CallsRevokeRefreshTokenAsync_AndReturnsUnit()
        {
            var result = await _handler.Handle(new LogoutRequest(), CancellationToken.None);

            _tokenServiceMock.Verify(x => x.RevokeRefreshTokenAsync(), Times.Once);
            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_WhenRevokeThrows_PropagatesException()
        {
            _tokenServiceMock
                .Setup(x => x.RevokeRefreshTokenAsync())
                .ThrowsAsync(new InvalidOperationException("Revoke failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(new LogoutRequest(), CancellationToken.None));
        }
    }
}