namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<List<Doctor>> SearchByNameAsync(string name);
    Task<List<Doctor>> FilterBySpecializationAsync(Guid specializationId);
    Task<List<Doctor>> FilterByOfficeAsync(Guid officeId);
    Task ChangeStatusAsync(Guid id, string status);
}
