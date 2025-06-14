using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface ISpecializationRepository : IRepository<Specialization>
{
    Task<List<Specialization>> FilterByNameAsync(string name);
}
