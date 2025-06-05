namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<List<Patient>> SearchByNameAsync(string name);
}