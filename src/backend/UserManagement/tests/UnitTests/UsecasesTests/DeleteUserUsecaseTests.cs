using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using MediatR;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.Models;
namespace UnitTests.UsecasesTests
{
    public class DeleteUserHandlerTests
    {
        private readonly Mock<IUserStore<Account>> _userStoreMock;
        private readonly Mock<UserManager<Account>> _userManagerMock;
        private readonly DeleteUserHandler _handler;

        public DeleteUserHandlerTests()
        {
            _userStoreMock = new Mock<IUserStore<Account>>();
            _userManagerMock = new Mock<UserManager<Account>>(
                _userStoreMock.Object,
                null, null, null, null, null, null, null, null
            );
            _handler = new DeleteUserHandler(_userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidUser_DeletesAndReturnsUnit()
        {
            var userId = Guid.NewGuid();
            var account = new Account { Id = userId };

            _userManagerMock
                .Setup(um => um.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(account);
            _userManagerMock
                .Setup(um => um.DeleteAsync(account))
                .ReturnsAsync(IdentityResult.Success);

            var request = new DeleteUserRequest(userId);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.Should().Be(Unit.Value);
            _userManagerMock.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
            _userManagerMock.Verify(um => um.DeleteAsync(account), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsInvalidOperationException()
        {
            var userId = Guid.NewGuid();
            _userManagerMock
                .Setup(um => um.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((Account)null);

            var request = new DeleteUserRequest(userId);

            Func<Task> act = () => _handler.Handle(request, CancellationToken.None);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Пользователь не найден.");
        }

        [Fact]
        public async Task Handle_DeleteFails_ThrowsExceptionWithErrors()
        {
            var userId = Guid.NewGuid();
            var account = new Account { Id = userId };

            var errors = new[]
            {
            new IdentityError { Code = "E1", Description = "Error1" },
            new IdentityError { Code = "E2", Description = "Error2" }
        };

            _userManagerMock
                .Setup(um => um.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(account);
            _userManagerMock
                .Setup(um => um.DeleteAsync(account))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var request = new DeleteUserRequest(userId);
            Func<Task> act = () => _handler.Handle(request, CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .Where(ex => ex.Message.Contains("Не удалось удалить пользователя: Error1, Error2"));
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            Func<Task> act = () => _handler.Handle(null, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}