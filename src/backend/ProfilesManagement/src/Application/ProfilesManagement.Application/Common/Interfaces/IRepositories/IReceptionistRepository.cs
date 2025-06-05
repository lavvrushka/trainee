namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IReceptionistRepository : IRepository<Receptionist>
{
    Task<List<Receptionist>> SearchByNameAsync(string name);
}