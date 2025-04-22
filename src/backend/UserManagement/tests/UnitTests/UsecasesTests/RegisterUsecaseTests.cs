using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagement.Application.UseCases.AuthUsecases;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Application.Tests.UseCases.Auth;

public class RegisterUserHandlerTests
{
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly Mock<RoleManager<Role>> _roleManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IEmailConfirmationService> _emailConfirmationServiceMock;
    private readonly RegisterUserHandler _handler;

    private const string DefaultRole = "PATIENT";
    private const string Email = "newuser@example.com";
    private const string Password = "Password123!";

    public RegisterUserHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<Account>>();
        _userManagerMock = new Mock<UserManager<Account>>(userStoreMock.Object,
            null, null, null, null, null, null, null, null);

        var roleStoreMock = new Mock<IRoleStore<Role>>();
        var loggerMock = new Mock<ILogger<RoleManager<Role>>>();
        _roleManagerMock = new Mock<RoleManager<Role>>(roleStoreMock.Object,
            new List<IRoleValidator<Role>>(), new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), loggerMock.Object);

        _tokenServiceMock = new Mock<ITokenService>();
        _emailConfirmationServiceMock = new Mock<IEmailConfirmationService>();

        _handler = new RegisterUserHandler(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _tokenServiceMock.Object,
            _emailConfirmationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ThrowsInvalidOperationException()
    {
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync(new Account { Email = Email });

        var request = new UserRegisterRequest(Email, Password, Password);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(request, CancellationToken.None));
        Assert.Contains("Пользователь с таким email уже зарегистрирован", ex.Message);
    }

    [Fact]
    public async Task Handle_CreateFails_ThrowsExceptionWithErrors()
    {
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((Account)null);

        var identityErrors = new[]
        {
            new IdentityError { Description = "Error1" },
            new IdentityError { Description = "Error2" }
        };
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        var request = new UserRegisterRequest(Email, Password, Password);

        var ex = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(request, CancellationToken.None));
        Assert.Contains("Ошибка создания пользователя: Error1, Error2", ex.Message);
    }

    [Fact]
    public async Task Handle_RoleNotExists_ThrowsException()
    {
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((Account)null);
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock
            .Setup(x => x.RoleExistsAsync(DefaultRole))
            .ReturnsAsync(false);

        var request = new UserRegisterRequest(Email, Password, Password);

        var ex = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(request, CancellationToken.None));
        Assert.Contains($"Роль {DefaultRole} не существует в системе", ex.Message);
    }

    [Fact]
    public async Task Handle_AddToRoleFails_ThrowsExceptionWithErrors()
    {
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((Account)null);
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock
            .Setup(x => x.RoleExistsAsync(DefaultRole))
            .ReturnsAsync(true);

        var roleErrors = new[] { new IdentityError { Description = "RoleError" } };
        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<Account>(), DefaultRole))
            .ReturnsAsync(IdentityResult.Failed(roleErrors));

        var request = new UserRegisterRequest(Email, Password, Password);

        var ex = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(request, CancellationToken.None));
        Assert.Contains("Ошибка назначения роли: RoleError", ex.Message);
    }

    [Fact]
    public async Task Handle_SuccessfulRegistration_ReturnsUserAuthResponse()
    {
        var userId = Guid.NewGuid();
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(Email))
            .ReturnsAsync((Account)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<Account>(), Password))
            .Callback<Account, string>((user, pwd) => user.Id = userId)
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock
            .Setup(x => x.RoleExistsAsync(DefaultRole))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<Account>(), DefaultRole))
            .ReturnsAsync(IdentityResult.Success);

        _emailConfirmationServiceMock
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(userId))
            .ReturnsAsync("email-token");
        _emailConfirmationServiceMock
            .Setup(x => x.SendConfirmationEmailAsync(userId))
            .Returns(Task.CompletedTask);

        var tokenPair = new TokensPair
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token"
        };
        _tokenServiceMock
            .Setup(x => x.GenerateTokensPairAsync(It.IsAny<Account>()))
            .ReturnsAsync(tokenPair);

        var request = new UserRegisterRequest(Email, Password, Password);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(tokenPair.AccessToken, result.AccessToken);
        Assert.Equal(tokenPair.RefreshToken, result.RefreshToken);

        _userManagerMock.Verify(x => x.CreateAsync(
            It.Is<Account>(u => u.Email == Email && u.UserName == Email && u.Id == userId), Password), Times.Once);
        _roleManagerMock.Verify(x => x.RoleExistsAsync(DefaultRole), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(
            It.Is<Account>(u => u.Id == userId), DefaultRole), Times.Once);
        _emailConfirmationServiceMock.Verify(x => x.GenerateEmailConfirmationTokenAsync(userId), Times.Once);
        _emailConfirmationServiceMock.Verify(x => x.SendConfirmationEmailAsync(userId), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateTokensPairAsync(
            It.Is<Account>(u => u.Id == userId)), Times.Once);
    }
}
