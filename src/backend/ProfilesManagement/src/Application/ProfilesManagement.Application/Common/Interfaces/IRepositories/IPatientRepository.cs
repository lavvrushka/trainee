namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
}