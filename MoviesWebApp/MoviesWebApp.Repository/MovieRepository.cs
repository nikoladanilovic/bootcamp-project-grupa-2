using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly string _connectionString;

        public MovieRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        private NpgsqlConnection CreateConnection() => new(_connectionString);

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            var movies = new List<Movie>();

            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT * FROM movies", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                movies.Add(MapToMovie(reader));
            }

            return movies;
        }

        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT * FROM movies WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToMovie(reader);
            }

            return null;
        }

        public async Task AddAsync(Movie movie)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(@"
            INSERT INTO movies (id, title, release_year, duration_minutes, director_id, description)
            VALUES (@id, @title, @release_year, @duration_minutes, @director_id, @description)", conn);

            cmd.Parameters.AddWithValue("id", movie.Id);
            cmd.Parameters.AddWithValue("title", movie.Title);
            cmd.Parameters.AddWithValue("release_year", movie.ReleaseYear);
            cmd.Parameters.AddWithValue("duration_minutes", movie.DurationMinutes);
            cmd.Parameters.AddWithValue("director_id", movie.DirectorId);
            cmd.Parameters.AddWithValue("description", (object?)movie.Description ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(@"
            UPDATE movies SET
                title = @title,
                release_year = @release_year,
                duration_minutes = @duration_minutes,
                director_id = @director_id,
                description = @description
            WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", movie.Id);
            cmd.Parameters.AddWithValue("title", movie.Title);
            cmd.Parameters.AddWithValue("release_year", movie.ReleaseYear);
            cmd.Parameters.AddWithValue("duration_minutes", movie.DurationMinutes);
            cmd.Parameters.AddWithValue("director_id", movie.DirectorId);
            cmd.Parameters.AddWithValue("description", (object?)movie.Description ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("DELETE FROM movies WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        private Movie MapToMovie(NpgsqlDataReader reader)
        {
            return new Movie
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ReleaseYear = reader.GetInt32(reader.GetOrdinal("release_year")),
                DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                DirectorId = reader.GetGuid(reader.GetOrdinal("director_id")),
                Description = reader.IsDBNull(reader.GetOrdinal("description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("description"))
            };
        }

        public async Task<IEnumerable<Movie>> GetMoviesFromDirector(Guid id)
        {
            var moviesList = new List<Movie>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand("SELECT * FROM movies WHERE director_id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    moviesList.Add(new Movie
                    {
                        Id = reader.GetGuid(0),
                        Title = reader.GetString(1),
                        ReleaseYear = reader.GetInt32(2),
                        DurationMinutes = reader.GetInt32(3),
                        DirectorId = reader.GetGuid(4),
                        Description = reader.IsDBNull(5) ? null : reader.GetString(5)
                    });
                }
            }
            return moviesList;
        }
    }
}
