using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MoviesWebApp.Service
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly ILogger<MovieService> _logger;

        public MovieService(IMovieRepository repository, ILogger<MovieService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            _logger.LogInformation("Get all available movies - service layer.");
            return await _repository.GetAllAsync();
        }
        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task AddAsync(Movie movie)
        {
            movie.Id = Guid.NewGuid(); // Assign new ID
            if (movie.ReleaseYear < 1800 || movie.ReleaseYear > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(movie.ReleaseYear), "Release year must be between 1800 and the current year.");
            }
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                throw new ArgumentException("Movie title cannot be null or empty.", nameof(movie.Title));
            }
            if (movie.DurationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(movie.DurationMinutes), "Duration must be a positive number.");
            }
            if (string.IsNullOrWhiteSpace(movie.Description))
            {
                throw new ArgumentException("Movie description cannot be null or empty.", nameof(movie.Title));
            }
            await _repository.AddAsync(movie);
        }
        public async Task UpdateAsync(Movie movie)
        {
            if (movie.ReleaseYear < 1800 || movie.ReleaseYear > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(movie.ReleaseYear), "Release year must be between 1800 and the current year.");
            }
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                throw new ArgumentException("Movie title cannot be null or empty.", nameof(movie.Title));
            }
            if (movie.DurationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(movie.DurationMinutes), "Duration must be a positive number.");
            }
            if (string.IsNullOrWhiteSpace(movie.Description))
            {
                throw new ArgumentException("Movie description cannot be null or empty.", nameof(movie.Title));
            }
            await _repository.UpdateAsync(movie);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<Movie> GetGenresOfMovieAsync(Guid id)
        {
            var movie = await _repository.GetGenresOfMovieAsync(id);
            if (movie == null || movie.Genres == null)
            {
                throw new KeyNotFoundException($"Movie with ID {id} not found.");
            }
            return movie;
        }
        public async Task<Movie> GetReviewsOfMovieAsync(Guid id)
        {
            var movie = await _repository.GetReviewsOfMovieAsync(id);
            if (movie == null || movie.Reviews == null)
            {
                throw new KeyNotFoundException($"Movie with ID {id} not found.");
            }
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesCuratedAsync(int releasedYearFilter, string ordering, int moviesPerPage, int page)
        {
            return await _repository.GetAllMoviesCuratedAsync(releasedYearFilter, ordering, moviesPerPage, page);
        }

        public async Task<int> GetCountOfAllMoviesAsync()
        {
            return await _repository.GetCountOfAllMoviesAsync();
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesWithDirectorsAndGenres(int releasedYearFilter, string ordering, int moviesPerPage, int page, string genre, string nameOfMovie)
        {
            IEnumerable<Movie> movies = await _repository.GetAllMoviesWithDirectorsAndGenres(releasedYearFilter, ordering, moviesPerPage, page, genre, nameOfMovie);

            // Select only movies that match the genre criteria
            if (!string.IsNullOrWhiteSpace(genre) && genre != "nothing")
            {
                movies = movies.Where(m => m.Genres.Any(g => g.Name.Equals(genre, StringComparison.OrdinalIgnoreCase)));
            }

            // Select only movies that match the name criteria
            if (!string.IsNullOrWhiteSpace(nameOfMovie) && nameOfMovie != "nothing")
            {
                movies = movies.Where(m => m.Title.Contains(nameOfMovie, StringComparison.OrdinalIgnoreCase));
            }


            return movies;
            
        }

        public async Task<int> GetMoviesCountWithFilters(int releasedYearFilter, string genre, string nameOfMovie)
        {
            return await _repository.GetMoviesCountWithFilters(releasedYearFilter, genre, nameOfMovie);
        }

        public async Task AddMovieWithGenresAsync(Movie movie)
        {
            if (movie.ReleaseYear < 1800 || movie.ReleaseYear > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(movie.ReleaseYear), "Release year must be between 1800 and the current year.");
            }
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                throw new ArgumentException("Movie title cannot be null or empty.", nameof(movie.Title));
            }
            if (movie.DurationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(movie.DurationMinutes), "Duration must be a positive number.");
            }
            if (string.IsNullOrWhiteSpace(movie.Description))
            {
                throw new ArgumentException("Movie description cannot be null or empty.", nameof(movie.Title));
            }
            await _repository.AddMovieWithGenresAsync(movie);
        }
    }
}
