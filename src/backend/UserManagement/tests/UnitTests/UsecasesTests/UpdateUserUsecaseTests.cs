using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.Tests.UseCases.User
{
    public class UpdateUserHandlerTestsFixture
    {
        public Mock<UserManager<Account>> UserManagerMock { get; }
        public Mock<ITokenService> TokenServiceMock { get; }
        public Mock<IEmailConfirmationService> EmailConfirmationServiceMock { get; }
        public UpdateUserHandler Handler { get; }

        public UpdateUserHandlerTestsFixture()
        {
            var userStore = new Mock<IUserStore<Account>>().Object;
            UserManagerMock = new Mock<UserManager<Account>>(
                userStore, null, null, null, null, null, null, null, null);

            TokenServiceMock = new Mock<ITokenService>();
            EmailConfirmationServiceMock = new Mock<IEmailConfirmationService>();

            Handler = new UpdateUserHandler(
                UserManagerMock.Object,
                TokenServiceMock.Object,
                EmailConfirmationServiceMock.Object);
        }
    }

    public class UpdateUserHandlerTests : IClassFixture<UpdateUserHandlerTestsFixture>
    {
        private readonly UpdateUserHandlerTestsFixture _f;
        private readonly Guid _userId = Guid.NewGuid();

        private const string OldEmail = "old@example.com";
        private const string NewEmail = "new@example.com";

        public UpdateUserHandlerTests(UpdateUserHandlerTestsFixture fixture)
        {
            _f = fixture;
            _f.UserManagerMock.Invocations.Clear();
            _f.TokenServiceMock.Invocations.Clear();
            _f.EmailConfirmationServiceMock.Invocations.Clear();
        }

        [Fact(DisplayName = "Invalid token → throws UnauthorizedAccessException")]
        public async Task Handle_InvalidToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _f.TokenServiceMock
              .Setup(x => x.GetUserIdFromAccessToken())
              .Returns(Guid.Empty);

            var request = new UpdateUserRequest(NewEmail);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid token.");
        }

        [Fact(DisplayName = "User not found → throws UnauthorizedAccessException")]
        public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _f.TokenServiceMock
              .Setup(x => x.GetUserIdFromAccessToken())
              .Returns(_userId);

            _f.UserManagerMock
              .Setup(x => x.FindByIdAsync(_userId.ToString()))
              .ReturnsAsync((Account)null);

            var request = new UpdateUserRequest(NewEmail);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User not found.");
        }

        [Fact(DisplayName = "Update fails → throws Exception with errors")]
        public async Task Handle_UpdateFails_ThrowsExceptionWithErrors()
        {
            // Arrange
            _f.TokenServiceMock
              .Setup(x => x.GetUserIdFromAccessToken())
              .Returns(_userId);

            var user = new Account { Id = _userId, Email = OldEmail, UserName = OldEmail };
            _f.UserManagerMock
              .Setup(x => x.FindByIdAsync(_userId.ToString()))
              .ReturnsAsync(user);

            var identityErrors = new[]
            {
                new IdentityError { Description = "Err1" },
                new IdentityError { Description = "Err2" }
            };
            _f.UserManagerMock
              .Setup(x => x.UpdateAsync(user))
              .ReturnsAsync(IdentityResult.Failed(identityErrors));

            var request = new UpdateUserRequest(NewEmail);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Failed to update user: Err1, Err2");
        }

        [Fact(DisplayName = "Email unchanged → succeeds without confirmation email")]
        public async Task Handle_EmailUnchanged_SucceedsWithoutConfirmationEmail()
        {
            // Arrange
            _f.TokenServiceMock
              .Setup(x => x.GetUserIdFromAccessToken())
              .Returns(_userId);

            var user = new Account
            {
                Id = _userId,
                Email = OldEmail,
                UserName = OldEmail,
                EmailVerifiedAt = DateTime.UtcNow
            };
            _f.UserManagerMock
              .Setup(x => x.FindByIdAsync(_userId.ToString()))
              .ReturnsAsync(user);

            _f.UserManagerMock
              .Setup(x => x.UpdateAsync(user))
              .ReturnsAsync(IdentityResult.Success);

            var request = new UpdateUserRequest(OldEmail);

            // Act
            var result = await _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
            _f.EmailConfirmationServiceMock
             .Verify(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<Guid>()), Times.Never);
            _f.EmailConfirmationServiceMock
             .Verify(x => x.SendConfirmationEmailAsync(It.IsAny<Guid>()), Times.Never);
            _f.UserManagerMock
             .Verify(x => x.UpdateAsync(
                 It.Is<Account>(u => u.Email == OldEmail && u.EmailVerifiedAt != null)), Times.Once);
        }

        [Fact(DisplayName = "Email changed → sends confirmation email and succeeds")]
        public async Task Handle_EmailChanged_SendsConfirmationEmailAndSucceeds()
        {
            // Arrange
            _f.TokenServiceMock
              .Setup(x => x.GetUserIdFromAccessToken())
              .Returns(_userId);

            var user = new Account
            {
                Id = _userId,
                Email = OldEmail,
                UserName = OldEmail,
                EmailVerifiedAt = DateTime.UtcNow
            };
            _f.UserManagerMock
              .Setup(x => x.FindByIdAsync(_userId.ToString()))
              .ReturnsAsync(user);

            const string newToken = "confirm-token";
            _f.EmailConfirmationServiceMock
              .Setup(x => x.GenerateEmailConfirmationTokenAsync(_userId))
              .ReturnsAsync(newToken);
            _f.EmailConfirmationServiceMock
              .Setup(x => x.SendConfirmationEmailAsync(_userId))
              .Returns(Task.CompletedTask);

            _f.UserManagerMock
              .Setup(x => x.UpdateAsync(user))
              .ReturnsAsync(IdentityResult.Success);

            var request = new UpdateUserRequest(NewEmail);

            // Act
            var result = await _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
            user.EmailVerifiedAt.Should().BeNull();
            user.EmailConfirmationToken.Should().Be(newToken);

            _f.EmailConfirmationServiceMock
             .Verify(x => x.GenerateEmailConfirmationTokenAsync(_userId), Times.Once);
            _f.EmailConfirmationServiceMock
             .Verify(x => x.SendConfirmationEmailAsync(_userId), Times.Once);
            _f.UserManagerMock
             .Verify(x => x.UpdateAsync(
                 It.Is<Account>(u => u.Email == NewEmail && u.UserName == NewEmail)), Times.Once);
        }
    }
}
