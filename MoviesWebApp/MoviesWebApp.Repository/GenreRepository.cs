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
        private readonly string _connectionString;
        public GenreRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
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

        public async Task<List<MovieGenre>> GetMovieGenresByGenreIdAsync(Guid genreId)
        {
            var movieGenres = new List<MovieGenre>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(
                    "SELECT mg.\"movie_id\", mg.\"genre_id\", " +
                    "m.\"id\", m.\"title\", m.\"release_year\", m.\"duration_minutes\", m.\"director_id\", m.\"description\", " +
                    "g.\"id\", g.\"name\" " +  
                    "FROM \"movie_genres\" mg " +
                    "INNER JOIN \"movies\" m ON mg.\"movie_id\" = m.\"id\" " +
                    "INNER JOIN \"genres\" g ON mg.\"genre_id\" = g.\"id\" " +
                    "WHERE mg.\"genre_id\" = @GenreId", conn))
                {
                    cmd.Parameters.AddWithValue("GenreId", genreId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var movie = new Movie
                            {
                                Id = reader.GetGuid(2),
                                Title = reader.GetString(3),
                                ReleaseYear = reader.GetInt32(4),
                                DurationMinutes = reader.GetInt32(5),
                                DirectorId = reader.GetGuid(6),
                                Description = reader.IsDBNull(7) ? null : reader.GetString(7)
                            };

                            var genre = new Genre
                            {
                                Id = reader.GetGuid(8),
                                Name = reader.GetString(9)
                            };

                            movieGenres.Add(new MovieGenre
                            {
                                MovieId = reader.GetGuid(0),
                                GenreId = reader.GetGuid(1),
                                Movie = movie,
                                Genre = genre
                            });
                        }
                    }
                }
            }

            if (movieGenres.Count == 0)
            {
                
                return movieGenres; 
            }

            return movieGenres;
        }
    }

}


