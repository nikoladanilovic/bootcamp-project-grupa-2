using MoviesWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Service.Common
{
    public interface IDirectorService
    {
        Task<IEnumerable<Director>> GetAllAsync();
        Task<Director?> GetByIdAsync(Guid id);
        Task AddAsync(Director director);
        Task UpdateAsync(Director director);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Movie>> GetMoviesFromDirector(Guid id);
    }
}
