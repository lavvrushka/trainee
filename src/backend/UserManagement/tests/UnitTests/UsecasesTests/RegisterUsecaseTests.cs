using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagement.Application.UseCases.AuthUsecases;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.Tests.UseCases.Auth
{
    public class RegisterUserHandlerTestsFixture
    {
        public Mock<UserManager<Account>> UserManagerMock { get; }
        public Mock<RoleManager<Role>> RoleManagerMock { get; }
        public Mock<ITokenService> TokenServiceMock { get; }
        public Mock<IEmailConfirmationService> EmailConfirmationServiceMock { get; }
        public RegisterUserHandler Handler { get; }

        public RegisterUserHandlerTestsFixture()
        {
            var userStore = new Mock<IUserStore<Account>>().Object;
            UserManagerMock = new Mock<UserManager<Account>>(
                userStore, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<Role>>().Object;
            RoleManagerMock = new Mock<RoleManager<Role>>(
                roleStore,
                new List<IRoleValidator<Role>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<Role>>>().Object);

            TokenServiceMock = new Mock<ITokenService>();
            EmailConfirmationServiceMock = new Mock<IEmailConfirmationService>();

            Handler = new RegisterUserHandler(
                UserManagerMock.Object,
                RoleManagerMock.Object,
                TokenServiceMock.Object,
                EmailConfirmationServiceMock.Object);
        }
    }

    public class RegisterUserHandlerTests : IClassFixture<RegisterUserHandlerTestsFixture>
    {
        private readonly RegisterUserHandlerTestsFixture _f;

        private const string Email = "newuser@example.com";
        private const string Password = "Password123!";
        private const string ConfirmPassword = "Password123!";
        private const string DefaultRole = "PATIENT";

        public RegisterUserHandlerTests(RegisterUserHandlerTestsFixture fixture)
        {
            _f = fixture;
            _f.UserManagerMock.Invocations.Clear();
            _f.RoleManagerMock.Invocations.Clear();
            _f.TokenServiceMock.Invocations.Clear();
            _f.EmailConfirmationServiceMock.Invocations.Clear();
        }

        [Theory(DisplayName = "Если email уже занят → бросается InvalidOperationException")]
        [InlineData("existing@example.com")]
        public async Task Handle_WhenUserExists_ThrowsInvalidOperationException(string existingEmail)
        {
            // Arrange
            _f.UserManagerMock
              .Setup(x => x.FindByEmailAsync(existingEmail))
              .ReturnsAsync(new Account { Email = existingEmail });

            var request = new UserRegisterRequest(existingEmail, Password, ConfirmPassword);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Пользователь с таким email уже зарегистрирован*");
        }

        [Theory(DisplayName = "Несовпадение паролей → бросается ArgumentException")]
        [InlineData("pass1", "pass2")]
        public async Task Handle_WhenPasswordsDoNotMatch_ThrowsArgumentException(string pwd, string confirm)
        {
            // Arrange
            var request = new UserRegisterRequest(Email, pwd, confirm);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*Пароли не совпадают*");
        }

        [Fact(DisplayName = "Ошибка создания пользователя при IdentityResult.Failed")]
        public async Task Handle_CreateFails_ShouldThrowExceptionWithErrors()
        {
            // Arrange
            _f.UserManagerMock
              .Setup(x => x.FindByEmailAsync(Email))
              .ReturnsAsync((Account)null);

            var identityErrors = new[]
            {
                new IdentityError { Description = "Error1" },
                new IdentityError { Description = "Error2" }
            };
            _f.UserManagerMock
              .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
              .ReturnsAsync(IdentityResult.Failed(identityErrors));

            var request = new UserRegisterRequest(Email, Password, ConfirmPassword);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"Ошибка создания пользователя: Error1, Error2");
        }

        [Fact(DisplayName = "Роль не найдена → бросается Exception")]
        public async Task Handle_RoleNotExists_ShouldThrowException()
        {
            // Arrange
            _f.UserManagerMock
              .Setup(x => x.FindByEmailAsync(Email))
              .ReturnsAsync((Account)null);
            _f.UserManagerMock
              .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
              .ReturnsAsync(IdentityResult.Success);
            _f.RoleManagerMock
              .Setup(r => r.RoleExistsAsync(DefaultRole))
              .ReturnsAsync(false);

            var request = new UserRegisterRequest(Email, Password, ConfirmPassword);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"Роль {DefaultRole} не существует в системе");
        }

        [Fact(DisplayName = "Ошибка назначения роли → бросается Exception с описанием")]
        public async Task Handle_AddToRoleFails_ShouldThrowExceptionWithErrors()
        {
            // Arrange
            _f.UserManagerMock
              .Setup(x => x.FindByEmailAsync(Email))
              .ReturnsAsync((Account)null);
            _f.UserManagerMock
              .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
              .ReturnsAsync(IdentityResult.Success);
            _f.RoleManagerMock
              .Setup(r => r.RoleExistsAsync(DefaultRole))
              .ReturnsAsync(true);

            var roleErrors = new[] { new IdentityError { Description = "RoleError" } };
            _f.UserManagerMock
              .Setup(x => x.AddToRoleAsync(It.IsAny<Account>(), DefaultRole))
              .ReturnsAsync(IdentityResult.Failed(roleErrors));

            var request = new UserRegisterRequest(Email, Password, ConfirmPassword);

            // Act
            Func<Task> act = () => _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            await act
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"Ошибка назначения роли: RoleError");
        }

        [Fact(DisplayName = "Успешная регистрация возвращает токены и отправляет письмо")]
        public async Task Handle_SuccessfulRegistration_ReturnsTokensAndSendsEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _f.UserManagerMock
              .Setup(x => x.FindByEmailAsync(Email))
              .ReturnsAsync((Account)null);
            _f.UserManagerMock
              .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
              .Callback<Account, string>((u, p) => u.Id = userId)
              .ReturnsAsync(IdentityResult.Success);
            _f.RoleManagerMock
              .Setup(r => r.RoleExistsAsync(DefaultRole))
              .ReturnsAsync(true);
            _f.UserManagerMock
              .Setup(u => u.AddToRoleAsync(It.IsAny<Account>(), DefaultRole))
              .ReturnsAsync(IdentityResult.Success);

            _f.EmailConfirmationServiceMock
              .Setup(s => s.GenerateEmailConfirmationTokenAsync(userId))
              .ReturnsAsync("email-token");
            _f.EmailConfirmationServiceMock
              .Setup(s => s.SendConfirmationEmailAsync(userId))
              .Returns(Task.CompletedTask);

            var tokens = new TokensPair { AccessToken = "A", RefreshToken = "R" };
            _f.TokenServiceMock
              .Setup(t => t.GenerateTokensPairAsync(It.IsAny<Account>()))
              .ReturnsAsync(tokens);

            var request = new UserRegisterRequest(Email, Password, ConfirmPassword);

            // Act
            var result = await _f.Handler.Handle(request, CancellationToken.None);

            // Assert
            result.AccessToken.Should().Be(tokens.AccessToken);
            result.RefreshToken.Should().Be(tokens.RefreshToken);

            _f.UserManagerMock.Verify(u => u.CreateAsync(
                It.Is<Account>(ac => ac.Email == Email && ac.UserName == Email && ac.Id == userId),
                Password), Times.Once);
            _f.RoleManagerMock.Verify(r => r.RoleExistsAsync(DefaultRole), Times.Once);
            _f.UserManagerMock.Verify(u => u.AddToRoleAsync(
                It.Is<Account>(ac => ac.Id == userId), DefaultRole), Times.Once);
            _f.EmailConfirmationServiceMock.Verify(s =>
                s.GenerateEmailConfirmationTokenAsync(userId), Times.Once);
            _f.EmailConfirmationServiceMock.Verify(s =>
                s.SendConfirmationEmailAsync(userId), Times.Once);
            _f.TokenServiceMock.Verify(t =>
                t.GenerateTokensPairAsync(It.Is<Account>(ac => ac.Id == userId)), Times.Once);
        }
    }
}
