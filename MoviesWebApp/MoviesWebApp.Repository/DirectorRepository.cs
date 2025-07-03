using Microsoft.Extensions.Logging;
using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DirectorRepository> _logger;
        public DirectorRepository(string connectionString, ILogger<DirectorRepository> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<Director>> GetAllAsync(string? search, int page, int pageSize)
        {
            var directorsList = new List<Director>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sb = new StringBuilder("SELECT * FROM directors");
                if (!string.IsNullOrWhiteSpace(search))
                    sb.Append(" WHERE name ILIKE @search");
                sb.Append(" ORDER BY name ASC LIMIT @pageSize OFFSET @offset");

                var command = new NpgsqlCommand(sb.ToString(), connection);
                if (!string.IsNullOrWhiteSpace(search))
                    command.Parameters.AddWithValue("@search", $"%{search}%");
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    directorsList.Add(new Director
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Birthdate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                        Nationality = reader.IsDBNull(3) ? null : reader.GetString(3)
                    });
                }
            }
            return directorsList;
        }

        public async Task<Director?> GetByIdAsync(Guid id)
        {
            //using var conn = CreateConnection();
            //var sql = "SELECT * FROM directors WHERE id = @Id";
            //return await conn.QuerySingleOrDefaultAsync<Director>(sql, new { Id = id });

            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM directors WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Director
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Birthdate = reader.GetDateTime(2),
                    Nationality = reader.GetString(3)
                };
            }

            return null;
        }

        public async Task AddAsync(Director director)
        {
            //using var conn = CreateConnection();
            //var sql = "INSERT INTO directors (id, name, birthdate, nationality) VALUES (@Id, @Name, @Birthdate, @Nationality)";
            //await conn.ExecuteAsync(sql, director);

            using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            var command = new NpgsqlCommand("INSERT INTO directors (id, name, birthdate, nationality) " +
                "VALUES (@Id, @Name, @Birthdate, @Nationality)", connection);

            command.Parameters.AddWithValue("@Id", director.Id);
            command.Parameters.AddWithValue("@Name", director.Name);
            command.Parameters.AddWithValue("@Birthdate", director.Birthdate);
            command.Parameters.AddWithValue("@Nationality", director.Nationality);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Director director)
        {
            //using var conn = CreateConnection();
            //var sql = @"UPDATE directors 
            //        SET name = @Name, birthdate = @Birthdate, nationality = @Nationality 
            //        WHERE id = @Id";
            //await conn.ExecuteAsync(sql, director);

            using var connection = new NpgsqlConnection(_connectionString);

            await connection.OpenAsync();

            var command = new NpgsqlCommand("UPDATE directors " +
                "SET name = @Name, birthdate = @Birthdate, nationality = @Nationality " +
                "WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@Id", director.Id);
            command.Parameters.AddWithValue("@Name", director.Name);
            command.Parameters.AddWithValue("@Birthdate", director.Birthdate);
            command.Parameters.AddWithValue("@Nationality", director.Nationality);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand("DELETE FROM directors WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
