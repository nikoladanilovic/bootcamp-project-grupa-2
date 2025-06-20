using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Service.Common;
using Microsoft.Extensions.Logging;

namespace MoviesWebApp.Service
{
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorRepository _repository;
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<DirectorService> _logger;

        // Constructor injection for the director repository
        public DirectorService(IDirectorRepository repository, IMovieRepository movieRepository, ILogger<DirectorService> logger)
        {
            _repository = repository;
            _movieRepository = movieRepository;
            _logger = logger;
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
            if (string.IsNullOrWhiteSpace(director.Name))
            {
                throw new ArgumentException("Director name cannot be null or empty.", nameof(director.Name));
            }
            if (director.Birthdate.HasValue && director.Birthdate.Value > DateTime.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(director.Birthdate), "Birthdate cannot be in the future.");
            }
            if (string.IsNullOrWhiteSpace(director.Nationality))
            {
                throw new ArgumentException("Director nationality cannot be null or empty.", nameof(director.Name));
            }
            await _repository.AddAsync(director);
        }

        public async Task UpdateAsync(Director director)
        {
            if (string.IsNullOrWhiteSpace(director.Name))
            {
                throw new ArgumentException("Director name cannot be null or empty.", nameof(director.Name));
            }
            if (director.Birthdate.HasValue && director.Birthdate.Value > DateTime.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(director.Birthdate), "Birthdate cannot be in the future.");
            }
            if (string.IsNullOrWhiteSpace(director.Nationality))
            {
                throw new ArgumentException("Director nationality cannot be null or empty.", nameof(director.Name));
            }
            await _repository.UpdateAsync(director);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<IEnumerable<Movie>> GetMoviesFromDirector(Guid id)
        {
            var director = await _repository.GetByIdAsync(id);
            if (director == null)
            {
                throw new KeyNotFoundException($"Director with ID {id} not found.");
            }
            return await _movieRepository.GetMoviesFromDirector(id);
        }
    }
}
