using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;

namespace MoviesWebApp.Service
{
 public class UsersService : IUsersService
    {
        public readonly IUsersRepository _usersRepository;
        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _usersRepository.GetAllUsersAsync();
        }
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _usersRepository.GetUserByIdAsync(id);
        }
        public async Task CreateUserAsync(List<User> users)
        {
             await _usersRepository.CreateUserAsync(users);
        }
        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            return await _usersRepository.UpdateUserAsync(id, user);
        }
        public async Task DeleteUserAsync(Guid id)
        {
            await _usersRepository.DeleteUserAsync(id);
        }
    }
}
