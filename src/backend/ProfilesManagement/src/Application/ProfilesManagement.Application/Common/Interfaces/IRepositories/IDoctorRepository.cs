namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<IEnumerable<Doctor>> SearchByNameAsync(string name);
    Task<IEnumerable<Doctor>> FilterBySpecializationAsync(Guid specializationId);
    Task<IEnumerable<Doctor>> FilterByOfficeAsync(Guid officeId);
    Task ChangeStatusAsync(Guid id, string status);
}
