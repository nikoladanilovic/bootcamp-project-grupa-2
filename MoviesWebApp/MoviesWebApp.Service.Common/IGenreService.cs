using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Model;

namespace MoviesWebApp.Service.Common
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllGenresAsync();
        Task AddGenreAsync(List<Genre> genres);
        Task UpdateGenreAsync(Guid id, Genre genre);
        Task DeleteGenreAsync(Guid id);
        Task<Genre> GetGenreByIdAsync(Guid id);
        Task<List<MovieGenre>> GetMovieGenresByGenreIdAsync(Guid genreId);
    }
}
