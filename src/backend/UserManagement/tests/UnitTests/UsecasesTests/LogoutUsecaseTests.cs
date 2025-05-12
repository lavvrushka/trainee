using FluentAssertions;
using MediatR;
using Moq;
using UserManagement.Application.UseCases.Auth;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.Tests.UseCases.Auth;

public class LogoutHandlerFixture
{
    public Mock<ITokenService> TokenServiceMock { get; }
    public LogoutHandler Handler { get; }

    public LogoutHandlerFixture()
    {
        TokenServiceMock = new Mock<ITokenService>();
        Handler = new LogoutHandler(TokenServiceMock.Object);
    }

    public void ResetMocks()
    {
        TokenServiceMock.Invocations.Clear();
        TokenServiceMock.Reset(); 
    }
}

public class LogoutHandlerTests : IClassFixture<LogoutHandlerFixture>
{
    private readonly LogoutHandlerFixture _fixture;

    public LogoutHandlerTests(LogoutHandlerFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks(); 
    }

    [Fact(DisplayName = "Logout → вызывает RevokeRefreshTokenAsync один раз и возвращает Unit")]
    public async Task Handle_CallsRevokeRefreshTokenAsync_AndReturnsUnit()
    {
        // Act
        var result = await _fixture.Handler.Handle(new LogoutRequest(), CancellationToken.None);

        // Assert
        _fixture.TokenServiceMock.Verify(x => x.RevokeRefreshTokenAsync(), Times.Once);
        result.Should().Be(Unit.Value);
    }

    [Fact(DisplayName = "Logout → если Revoke выбрасывает исключение, оно пробрасывается")]
    public async Task Handle_WhenRevokeThrows_PropagatesException()
    {
        // Arrange
        _fixture.TokenServiceMock
            .Setup(x => x.RevokeRefreshTokenAsync())
            .ThrowsAsync(new InvalidOperationException("Revoke failed"));

        // Act
        Func<Task> act = () => _fixture.Handler.Handle(new LogoutRequest(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Revoke failed");
    }
}
