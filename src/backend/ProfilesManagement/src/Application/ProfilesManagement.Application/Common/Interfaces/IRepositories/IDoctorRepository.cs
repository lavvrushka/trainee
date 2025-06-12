using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    public Task<List<Doctor>> FilterByNameAsync(string name);
    public Task<List<Doctor>> FilterBySpecializationAsync(Guid specializationId);
    public Task<List<Doctor>> FilterByOfficeAsync(Guid officeId);
    public Task ChangeStatusAsync(Guid id, string status);
    public Task<List<Doctor>> GetByPageAsync(PageSettings pageSettings);
}
