using MoviesWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository.Common
{
    public interface IDirectorRepository
    {
        Task<IEnumerable<Director>> GetAllAsync();
        Task<Director?> GetByIdAsync(Guid id);
        Task AddAsync(Director director);
        Task UpdateAsync(Director director);
        Task DeleteAsync(Guid id);
    }
}
