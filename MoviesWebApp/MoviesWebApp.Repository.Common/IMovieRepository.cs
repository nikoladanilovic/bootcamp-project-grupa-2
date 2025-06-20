using MoviesWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository.Common
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie?> GetByIdAsync(Guid id);
        Task AddAsync(Movie movie);
        Task UpdateAsync(Movie movie);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Movie>> GetMoviesFromDirector(Guid id);
        Task<Movie> GetGenresOfMovieAsync(Guid id);
        Task<Movie> GetReviewsOfMovieAsync(Guid id);
        Task<IEnumerable<Movie>> GetAllMoviesCuratedAsync(int releasedYearFilter, string ordering, int moviesPerPage, int page);
    }
}
