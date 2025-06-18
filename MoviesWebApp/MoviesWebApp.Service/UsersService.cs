using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using BCrypt.Net;

namespace MoviesWebApp.Service
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _usersRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _usersRepository.GetUserByIdAsync(id);
        }

        public async Task CreateUserAsync(List<User> users)
        {
            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.Username))
                    throw new ArgumentException("Username is required.");
                if (string.IsNullOrEmpty(user.Email))
                    throw new ArgumentException("Email is required.");
                if (string.IsNullOrEmpty(user.Password))
                    throw new ArgumentException("Password is required.");

                if (await _usersRepository.UsernameExistsAsync(user.Username))
                    throw new InvalidOperationException($"Username '{user.Username}' already exists.");
                if (await _usersRepository.EmailExistsAsync(user.Email))
                    throw new InvalidOperationException($"Email '{user.Email}' already exists.");

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.CreatedAt = DateTime.UtcNow;
            }
            await _usersRepository.CreateUserAsync(users);
        }

        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("Username is required.");
            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentException("Email is required.");

            var existingUser = await _usersRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                return false;

            if (user.Username != existingUser.Username && await _usersRepository.UsernameExistsAsync(user.Username))
                throw new InvalidOperationException($"Username '{user.Username}' already exists.");

            if (user.Email != existingUser.Email && await _usersRepository.EmailExistsAsync(user.Email))
                throw new InvalidOperationException($"Email '{user.Email}' already exists.");

            if (!string.IsNullOrEmpty(user.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            else
                user.Password = existingUser.Password; 

            return await _usersRepository.UpdateUserAsync(id, user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _usersRepository.DeleteUserAsync(id);
        }
    }
}