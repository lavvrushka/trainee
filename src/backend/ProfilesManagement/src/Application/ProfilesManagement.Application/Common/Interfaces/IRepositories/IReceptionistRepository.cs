using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IReceptionistRepository : IRepository<Receptionist>
{
    public Task<List<Receptionist>> SearchByNameAsync(string name);
}