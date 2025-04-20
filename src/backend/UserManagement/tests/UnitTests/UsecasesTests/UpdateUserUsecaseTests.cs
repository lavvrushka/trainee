using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.Tests.UseCases.User
{
    public class UpdateUserHandlerTests
    {
        private readonly Mock<UserManager<Account>> _userManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IEmailConfirmationService> _emailConfirmationServiceMock;
        private readonly UpdateUserHandler _handler;
        private readonly Guid _userId;

        public UpdateUserHandlerTests()
        {
            var userStoreMock = new Mock<IUserStore<Account>>();
            _userManagerMock = new Mock<UserManager<Account>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);
            _tokenServiceMock = new Mock<ITokenService>();
            _emailConfirmationServiceMock = new Mock<IEmailConfirmationService>();

            _handler = new UpdateUserHandler(
                _userManagerMock.Object,
                _tokenServiceMock.Object,
                _emailConfirmationServiceMock.Object);

            _userId = Guid.NewGuid();
        }

        [Fact]
        public async Task Handle_InvalidToken_ThrowsUnauthorizedAccessException()
        {
            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(Guid.Empty);
            var request = new UpdateUserRequest("new@example.com");

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Invalid token.", ex.Message);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(_userId);
            _userManagerMock.Setup(x => x.FindByIdAsync(_userId.ToString()))
                .ReturnsAsync((Account)null);
            var request = new UpdateUserRequest("new@example.com");

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task Handle_UpdateFails_ThrowsExceptionWithErrors()
        {
            var oldEmail = "old@example.com";
            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(_userId);
            var user = new Account { Id = _userId, Email = oldEmail, UserName = oldEmail };
            _userManagerMock.Setup(x => x.FindByIdAsync(_userId.ToString()))
                .ReturnsAsync(user);
            var identityErrors = new[]
            {
                new IdentityError { Description = "Err1" },
                new IdentityError { Description = "Err2" }
            };
            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            var request = new UpdateUserRequest("new@example.com");

            var ex = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(request, CancellationToken.None));
            Assert.Contains("Failed to update user: Err1, Err2", ex.Message);
        }

        [Fact]
        public async Task Handle_EmailUnchanged_SucceedsWithoutConfirmationEmail()
        {
            var email = "same@example.com";
            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(_userId);
            var user = new Account { Id = _userId, Email = email, UserName = email, EmailVerifiedAt = DateTime.UtcNow };
            _userManagerMock.Setup(x => x.FindByIdAsync(_userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);
            var request = new UpdateUserRequest(email);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(Unit.Value, result);
            _emailConfirmationServiceMock.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<Guid>()), Times.Never);
            _emailConfirmationServiceMock.Verify(x => x.SendConfirmationEmailAsync(It.IsAny<Guid>()), Times.Never);
            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<Account>(u => u.Email == email)), Times.Once);
        }

        [Fact]
        public async Task Handle_EmailChanged_SendsConfirmationEmailAndSucceeds()
        {
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";
            _tokenServiceMock.Setup(x => x.GetUserIdFromAccessToken())
                .Returns(_userId);
            var user = new Account { Id = _userId, Email = oldEmail, UserName = oldEmail, EmailVerifiedAt = DateTime.UtcNow };
            var newToken = "confirm-token";
            _userManagerMock.Setup(x => x.FindByIdAsync(_userId.ToString()))
                .ReturnsAsync(user);
            _emailConfirmationServiceMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(_userId))
                .ReturnsAsync(newToken);
            _emailConfirmationServiceMock.Setup(x => x.SendConfirmationEmailAsync(_userId))
                .Returns(Task.CompletedTask);
            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var request = new UpdateUserRequest(newEmail);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(Unit.Value, result);
            Assert.Null(user.EmailVerifiedAt);
            Assert.Equal(newToken, user.EmailConfirmationToken);
            _emailConfirmationServiceMock.Verify(x => x.GenerateEmailConfirmationTokenAsync(_userId), Times.Once);
            _emailConfirmationServiceMock.Verify(x => x.SendConfirmationEmailAsync(_userId), Times.Once);
            _userManagerMock.Verify(x => x.UpdateAsync(It.Is<Account>(u => u.Email == newEmail)), Times.Once);
        }
    }
}
