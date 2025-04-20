using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserManagement.Application.Common.Mappings;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.UseCases.UserUsecases;
using UserManagement.Domain.Interfaces.Models;
using SharedTests;

public class GetAllUsersHandlerTests
{
    private readonly Mock<IUserStore<Account>> _userStoreMock;
    private readonly Mock<UserManager<Account>> _userManagerMock;
    private readonly GetAllUsersHandler _handler;

    public GetAllUsersHandlerTests()
    {
        _userStoreMock = new Mock<IUserStore<Account>>();
        _userManagerMock = new Mock<UserManager<Account>>(
            _userStoreMock.Object,
            null, null, null, null, null, null, null, null
        );
        _handler = new GetAllUsersHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUsersExist_ReturnsMappedUserDtos()
    {
        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid(), UserName = "Ivan", Email = "Ivan@test.com" },
            new Account { Id = Guid.NewGuid(), UserName = "Misha",   Email = "Misha@test.com"   }
        };

        var asyncUsers = new TestAsyncEnumerable<Account>(accounts);
        _userManagerMock
            .SetupGet(x => x.Users)
            .Returns(asyncUsers);

        var request = new GetAllUsersRequest();

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should()
              .BeEquivalentTo(accounts.Select(a => a.MapToUserDto()),
                              opts => opts.ComparingByMembers<UserDto>());
    }

    [Fact]
    public async Task Handle_WhenNoUsers_ThrowsException()
    {
        var emptyUsers = new TestAsyncEnumerable<Account>(Enumerable.Empty<Account>());
        _userManagerMock
            .SetupGet(x => x.Users)
            .Returns(emptyUsers);

        var request = new GetAllUsersRequest();

        Func<Task> act = () => _handler.Handle(request, CancellationToken.None);

        await act.Should()
                 .ThrowAsync<Exception>()
                 .WithMessage("No users found.");
    }
}
