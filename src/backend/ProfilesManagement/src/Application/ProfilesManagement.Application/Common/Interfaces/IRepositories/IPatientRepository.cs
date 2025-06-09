using Microsoft.Extensions.Logging;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.Common.Interfaces.IRepositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<List<Patient>> SearchByNameAsync(string name);
    Task<List<Patient>> GetByPageAsync(PageSettings pageSettings);
}