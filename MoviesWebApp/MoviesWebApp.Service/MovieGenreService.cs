using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Model;

namespace MoviesWebApp.Service
{
    public class MovieGenreService : IMovieGenreService
    {
        private readonly IMovieGenreRepository _movieGenreRepository;

        public MovieGenreService(IMovieGenreRepository movieGenreRepository)
        {
            _movieGenreRepository = movieGenreRepository;
        }
        public async Task<List<Guid>> GetMovieGenreIdsAsync(Guid movieId)
        {
            return await _movieGenreRepository.GetMovieGenreIdsAsync(movieId);
        }
        public async Task AddMovieGenreAsync(Guid movieId, Guid genreId)
        {
            await _movieGenreRepository.AddMovieGenreAsync(movieId, genreId);
        }
        public async Task DeleteMovieGenreAsync(Guid movieId, Guid genreId)
        {
            await _movieGenreRepository.DeleteMovieGenreAsync(movieId, genreId);
        }
        public async Task ClearMovieGenresAsync(Guid movieId)
        {
            await _movieGenreRepository.ClearMovieGenresAsync(movieId);

        }
        public async Task UpdateMovieGenreAsync(Guid movieId, Guid genreId)
        {
            await _movieGenreRepository.UpdateMovieGenreAsync(movieId, genreId);
        }
    }
}
