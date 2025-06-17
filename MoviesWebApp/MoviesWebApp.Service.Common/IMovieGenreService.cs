using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Service.Common
{
    public interface IMovieGenreService
    {
        Task<List<Guid>> GetMovieGenreIdsAsync(Guid movieId);
        Task AddMovieGenreAsync(Guid movieId, Guid genreId);
        Task DeleteMovieGenreAsync(Guid movieId, Guid genreId);
        Task ClearMovieGenresAsync(Guid movieId);
        Task UpdateMovieGenreAsync(Guid movieId, Guid genreId);
    }
}
