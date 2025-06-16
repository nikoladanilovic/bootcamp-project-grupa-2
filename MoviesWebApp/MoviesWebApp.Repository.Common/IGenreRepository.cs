using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Model;   

namespace MoviesWebApp.Repository.Common
{
    public interface IGenreRepository
    {

        Task<List<Genre>> GetAllGenresAsync();
        Task AddGenreAsync(List<Genre> genres);
        Task UpdateGenreAsync(Guid id, Genre genres);
        Task DeleteGenreAsync(Guid id);
        Task<Genre> GetGenreByIdAsync(Guid id);

    }
}
