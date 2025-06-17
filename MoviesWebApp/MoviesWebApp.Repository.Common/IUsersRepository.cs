using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Model;

namespace MoviesWebApp.Repository.Common
{
   public interface IUsersRepository
    {
        public Task<IEnumerable<User>> GetAllUsersAsync();
        public Task<User?> GetUserByIdAsync(Guid id);
        public Task CreateUserAsync(List<User> users);
        public Task <bool>UpdateUserAsync(Guid id, User user);
        public Task DeleteUserAsync(Guid id);
    }
}
