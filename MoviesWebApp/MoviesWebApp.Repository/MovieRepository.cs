using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MovieRepository> _logger;
        public MovieRepository(string connectionString, ILogger<MovieRepository> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }
        private NpgsqlConnection CreateConnection() => new(_connectionString);

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            //example of logging in repository layer
            _logger.LogInformation("Get all available movies - repository layer.");

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
        public async Task<Movie> GetGenresOfMovieAsync(Guid id)
        {
            Movie movieWithGenres = new Movie();
            movieWithGenres.Genres = new List<Genre>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand("select m.id, m.title, m.release_year, m.duration_minutes, m.director_id, m.description, g.id, g.name " +
                    "from movies m " +
                    "join movie_genres mg on m.id = mg.movie_id " +
                    "join genres g on g.id = mg.genre_id " +
                    "where mg.movie_id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    movieWithGenres.Id = reader.GetGuid(0);
                    movieWithGenres.Title = reader.GetString(1);
                    movieWithGenres.ReleaseYear = reader.GetInt32(2);
                    movieWithGenres.DurationMinutes = reader.GetInt32(3);
                    movieWithGenres.DirectorId = reader.GetGuid(4);
                    movieWithGenres.Description = reader.IsDBNull(5) ? null : reader.GetString(5);
                    movieWithGenres.Genres.Add(new Genre
                    {
                        Id = reader.GetGuid(6),
                        Name = reader.GetString(7)
                    });
                }
            }
            return movieWithGenres;
        }

        public async Task<Movie> GetReviewsOfMovieAsync(Guid id)
        {
            Movie movieWithReviews = new Movie();
            movieWithReviews.Reviews = new List<Review>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand("select m.id, m.title, m.release_year, m.duration_minutes, m.director_id, m.description, r.id, r.user_id, r.movie_id, r.rating, r.comment " +
                    "from movies m " +
                    "join reviews r on m.id = r.movie_id " +
                    "where r.movie_id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    movieWithReviews.Id = reader.GetGuid(0);
                    movieWithReviews.Title = reader.GetString(1);
                    movieWithReviews.ReleaseYear = reader.GetInt32(2);
                    movieWithReviews.DurationMinutes = reader.GetInt32(3);
                    movieWithReviews.DirectorId = reader.GetGuid(4);
                    movieWithReviews.Description = reader.IsDBNull(5) ? null : reader.GetString(5);
                    movieWithReviews.Reviews.Add(new Review
                    {
                        Id = reader.GetGuid(6),
                        UserId = reader.GetGuid(7),
                        MovieId = reader.GetGuid(8),
                        Rating = reader.GetInt32(9),
                        Comment = reader.IsDBNull(10) ? null : reader.GetString(10)
                    });
                }
            }
            return movieWithReviews;
        }
    }
}
