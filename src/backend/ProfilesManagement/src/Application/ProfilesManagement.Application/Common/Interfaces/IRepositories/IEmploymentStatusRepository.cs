using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IEmploymentStatusRepository : IRepository<EmploymentStatus>
{
    public Task<List<EmploymentStatus>> FilterByStatusNameAsync(string name);
}