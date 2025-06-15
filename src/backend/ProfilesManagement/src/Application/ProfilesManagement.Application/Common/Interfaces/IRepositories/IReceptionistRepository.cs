using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IReceptionistRepository : IRepository<Receptionist>
{
    public Task<List<Receptionist>> FilterByNameAsync(string Name);
    public Task<List<Receptionist>> GetByPageAsync(PageSettings pageSettings);
}