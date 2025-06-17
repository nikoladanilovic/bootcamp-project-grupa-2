using MoviesWebApp.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Model;
using Npgsql;
using System.Numerics;


namespace MoviesWebApp.Repository
{
    public class UsersRepository : IUsersRepository
    {
       private readonly string _connectionString ;

        public UsersRepository(string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=admin1235;Database=bootcamp-project")
        {
            _connectionString = connectionString;
        }
        
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"users\"", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetGuid(0),
                            Username = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Password = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CreatedAt = reader.GetDateTime(3)
                        });
                    }
                }
            }
            return users;

        }
        public async Task<User> GetUserByIdAsync(Guid id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"users\" WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetGuid(0),
                                Username = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3),
                                Password = reader.IsDBNull(4) ? null : reader.GetString(4)
                            };

                        }
                    }
                }
            }
            return null;

        }
        public async Task CreateUserAsync(List<User> users)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO \"users\" (\"id\", \"username\", \"email\", \"password\", \"created_at\") VALUES (@Id, @Username, @Email, @Password, @CreatedAt)", conn))
                {
                    foreach (var user in users)
                    {
                        user.Id = Guid.NewGuid();
                        cmd.Parameters.AddWithValue("@Id", user.Id);
                        cmd.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Password", user.Password ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        using (var reader = await cmd.ExecuteReaderAsync()) ;
                    }
                }   
            }

        }
        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE \"users\" " +
                    "SET \"username\" = @Username, \"email\" = @Email, \"password\" = @Password " +
                    "WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Username", (object?)user.Username ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", (object?)user.Password ?? DBNull.Value);

                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }


         public async Task DeleteUserAsync(Guid id){

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM \"users\" WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

        }

    }
}
