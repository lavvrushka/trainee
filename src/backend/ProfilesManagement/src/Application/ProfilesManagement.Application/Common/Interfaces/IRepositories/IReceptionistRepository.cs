namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IReceptionistRepository : IRepository<Receptionist>
{
    Task<IEnumerable<Receptionist>> SearchByNameAsync(string name);
}