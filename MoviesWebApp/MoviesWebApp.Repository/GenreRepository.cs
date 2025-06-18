using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository
{
    public class GenreRepository : IGenreRepository
    {

        private readonly string _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=admin1235;Database=bootcamp-project";

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            var genres = new List<Genre>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "SELECT g.\"id\", g.\"name\"" +
                    "FROM \"genres\"g", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var genre = new Genre
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),

                        };
                        genres.Add(genre);
                    }
                }

            }
            return genres;

        }
    
        public async Task AddGenreAsync(List<Genre> genres)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                foreach (var genre in genres)
                {
                    using (var cmd = new NpgsqlCommand(
                        "INSERT INTO \"genres\" (\"id\", \"name\") VALUES (@Id, @Name)", conn))
                    {
                        genre.Id = Guid.NewGuid(); 
                        cmd.Parameters.AddWithValue("Id", genre.Id);
                        cmd.Parameters.AddWithValue("Name", genre.Name);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }
        public async Task UpdateGenreAsync(Guid id, Genre genre)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE \"genres\" SET \"name\" = @Name WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.AddWithValue("Name", genre.Name);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task DeleteGenreAsync(Guid id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "DELETE FROM \"genres\" WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<Genre> GetGenreByIdAsync(Guid id)
        {
            string message = "Genre not found";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "SELECT g.\"id\", g.\"name\" " +
                    "FROM \"genres\" g " +
                    "WHERE g.\"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Genre
                            {

                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                            };
                        }
                        else
                        {
                            throw new Exception(message);
                        }
                    }
                }
            }
            return null;
        }
    }
}


