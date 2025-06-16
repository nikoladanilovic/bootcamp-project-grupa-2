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

        // Constructor injection for the director repository
        public DirectorService(IDirectorRepository repository)
        {
            _repository = repository;
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
    }
}
