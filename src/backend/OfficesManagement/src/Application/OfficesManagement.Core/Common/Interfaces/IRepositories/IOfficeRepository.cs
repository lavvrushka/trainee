using OfficesManagement.Core.Models;
using OfficesManagement.Core.Models.Entities;
using System.Linq.Expressions;
namespace OfficesManagement.Core.Common.Interfaces.IRepositories;
public interface IOfficeRepository : IRepository<Office>
{
    Task<List<Office>> GetActiveOfficesAsync();
    Task<List<Office>> GetOfficesByCityAsync(string city);
    Task<List<Office>> GetOfficesByStateAsync(string state);
    Task<List<Office>> GetOfficesByCountryAsync(string country);
    Task<Office?> GetOfficeByNameAsync(string name);
    Task<List<Office>> GetFilteredAsync(Expression<Func<Office, bool>> filter);
    Task<List<Office>> GetPageAsync(PageSettings pageSettings);
    Task<int> GetOfficeCountAsync();
}
