using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Service.Common;

namespace MoviesWebApp.Service
{
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorRepository _repository;
        private readonly IMovieRepository _movieRepository;

        // Constructor injection for the director repository
        public DirectorService(IDirectorRepository repository, IMovieRepository movieRepository)
        {
            _repository = repository;
            _movieRepository = movieRepository;
        }

        public async Task<IEnumerable<Director>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Director?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task AddAsync(Director director)
        {
            director.Id = Guid.NewGuid(); // Ensure ID is set
            await _repository.AddAsync(director);
        }

        public async Task UpdateAsync(Director director)
        {
            await _repository.UpdateAsync(director);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<IEnumerable<Movie>> GetMoviesFromDirector(Guid id)
        {
            return await _movieRepository.GetMoviesFromDirector(id);
        }
    }
}
