using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.Models;
using Xunit;

namespace UnitTests.UsecasesTests;

public class DeleteUserHandlerTests : IClassFixture<DeleteUserHandlerTests.Fixture>
{
    private readonly Fixture _fixture;

    public DeleteUserHandlerTests(Fixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }

    [Fact(DisplayName = "DeleteUser → при валидном ID пользователь удаляется и возвращается Unit")]
    public async Task Handle_WithValidUser_DeletesAndReturnsUnit()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var account = new Account { Id = userId };
        var request = new DeleteUserRequest(userId);

        _fixture.UserManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(account);

        _fixture.UserManagerMock
            .Setup(x => x.DeleteAsync(account))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _fixture.Handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _fixture.UserManagerMock.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
        _fixture.UserManagerMock.Verify(x => x.DeleteAsync(account), Times.Once);
    }

    [Fact(DisplayName = "DeleteUser → если пользователь не найден, выбрасывается InvalidOperationException")]
    public async Task Handle_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new DeleteUserRequest(userId);

        _fixture.UserManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((Account)null);

        // Act
        Func<Task> act = () => _fixture.Handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Пользователь не найден.");
    }

    [Fact(DisplayName = "DeleteUser → если удаление не удалось, выбрасывается Exception с деталями ошибок")]
    public async Task Handle_DeleteFails_ThrowsExceptionWithErrors()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var account = new Account { Id = userId };
        var request = new DeleteUserRequest(userId);

        var errors = new[]
        {
            new IdentityError { Code = "E1", Description = "Error1" },
            new IdentityError { Code = "E2", Description = "Error2" }
        };

        _fixture.UserManagerMock
            .Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(account);

        _fixture.UserManagerMock
            .Setup(x => x.DeleteAsync(account))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        Func<Task> act = () => _fixture.Handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .Where(ex => ex.Message.Contains("Не удалось удалить пользователя: Error1, Error2"));
    }

    [Fact(DisplayName = "DeleteUser → если запрос null, выбрасывается ArgumentNullException")]
    public async Task Handle_NullRequest_ThrowsArgumentNullException()
    {
        // Act
        Func<Task> act = () => _fixture.Handler.Handle(null, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    public class Fixture
    {
        public Mock<IUserStore<Account>> UserStoreMock { get; }
        public Mock<UserManager<Account>> UserManagerMock { get; }
        public DeleteUserHandler Handler { get; }

        public Fixture()
        {
            UserStoreMock = new Mock<IUserStore<Account>>();
            UserManagerMock = new Mock<UserManager<Account>>(
                UserStoreMock.Object, null, null, null, null, null, null, null, null);

            Handler = new DeleteUserHandler(UserManagerMock.Object);
        }

        public void ResetMocks()
        {
            UserManagerMock.Invocations.Clear();
            UserManagerMock.Reset();
        }
    }
}
