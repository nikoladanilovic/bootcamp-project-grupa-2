using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesWebApp.Service
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _repository;

        public ActorService(IActorRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Actor>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Actor?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Actor actor)
        {
            actor.Id = Guid.NewGuid();
            await _repository.AddAsync(actor);
        }

        public async Task UpdateAsync(Actor actor)
        {
            await _repository.UpdateAsync(actor);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}