using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Model;
using MoviesWebApp.Service.Common;
using MoviesWebApp.Repository.Common;


namespace MoviesWebApp.Service
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllGenresAsync();
        }
        public async Task AddGenreAsync(List<Genre> genres)
        {
            await _genreRepository.AddGenreAsync(genres);
        }
        public async Task UpdateGenreAsync(Guid id, Genre genre)
        {
            await _genreRepository.UpdateGenreAsync(id, genre);
        }
        public async Task DeleteGenreAsync(Guid id)
        {
            await _genreRepository.DeleteGenreAsync(id);
        }
        public async Task<Genre> GetGenreByIdAsync(Guid id)
        {
            return await _genreRepository.GetGenreByIdAsync(id);

        }
        public async Task<List<MovieGenre>> GetMovieGenresByGenreIdAsync(Guid genreId)
        {
            return await _genreRepository.GetMovieGenresByGenreIdAsync(genreId);
        }
    }
}
