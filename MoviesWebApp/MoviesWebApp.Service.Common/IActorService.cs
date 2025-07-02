using MoviesWebApp.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesWebApp.Service.Common
{
    public interface IActorService
    {
        Task<IEnumerable<Actor>> GetAllAsync();
        Task<Actor?> GetByIdAsync(Guid id);
        Task AddAsync(Actor actor);
        Task UpdateAsync(Actor actor);
        Task DeleteAsync(Guid id);
    }
}