using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using Npgsql;

namespace MoviesWebApp.Repository
{
    public class MovieGenreRepository : IMovieGenreRepository
    {
        private readonly string _connectionString;
        public MovieGenreRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        public async Task<List<Guid>> GetMovieGenreIdsAsync(Guid movieId)
        {
            var genreIds = new List<Guid>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "SELECT \"genre_id\" FROM \"movie_genres\" WHERE \"movie_id\" = @MovieId", conn))
                {
                    cmd.Parameters.AddWithValue("MovieId", movieId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            genreIds.Add(reader.GetGuid(0));
                        }
                    }
                }
            }
            return genreIds;
        }

        public async Task AddMovieGenreAsync(Guid movieId, Guid genreId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO \"movie_genres\" (\"movie_id\", \"genre_id\") VALUES (@MovieId, @GenreId)", conn))
                {
                    cmd.Parameters.AddWithValue("MovieId", movieId);
                    cmd.Parameters.AddWithValue("GenreId", genreId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteMovieGenreAsync(Guid movieId, Guid genreId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "DELETE FROM \"movie_genres\" WHERE \"movie_id\" = @MovieId AND \"genre_id\" = @GenreId", conn))
                {
                    cmd.Parameters.AddWithValue("MovieId", movieId);
                    cmd.Parameters.AddWithValue("GenreId", genreId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ClearMovieGenresAsync(Guid movieId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "DELETE FROM \"movie_genres\" WHERE \"movie_id\" = @MovieId", conn))
                {
                    cmd.Parameters.AddWithValue("MovieId", movieId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateMovieGenreAsync(Guid movieId, Guid genreId)
        {
            
            await ClearMovieGenresAsync(movieId);
            await AddMovieGenreAsync(movieId, genreId);
        }
    }
}
