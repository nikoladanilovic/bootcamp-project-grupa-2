using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Service
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;

        public MovieService(IMovieRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task AddAsync(Movie movie)
        {
            movie.Id = Guid.NewGuid(); // Assign new ID
            await _repository.AddAsync(movie);
        }
        public async Task UpdateAsync(Movie movie)
        {
            await _repository.UpdateAsync(movie);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
