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
        private readonly string _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=plava123;Database=bootcamp-project";

        private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<Director>> GetAllAsync()
        {
            //using var conn = CreateConnection();
            //var sql = "SELECT * FROM directors";
            //return await conn.QueryAsync<Director>(sql);

            var directorsList = new List<Director>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new NpgsqlCommand("SELECT * FROM directors", connection);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    directorsList.Add(new Director
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Birthdate = reader.GetDateTime(2),
                        Nationality = reader.GetString(3)
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
