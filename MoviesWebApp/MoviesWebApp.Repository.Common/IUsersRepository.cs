using MoviesWebApp.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository.Common
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task CreateUserAsync(List<User> users);
        Task<bool> UpdateUserAsync(Guid id, User user);
        Task DeleteUserAsync(Guid id);
    }
}
