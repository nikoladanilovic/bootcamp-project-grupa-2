using Microsoft.Extensions.Logging;
using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<IEnumerable<Movie>> GetAllMoviesCuratedAsync(int releasedYearFilter, string ordering, int moviesPerPage, int page)
        {

            var movies = new List<Movie>();

            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT * FROM movies m " +
                "WHERE m.release_year >= " + releasedYearFilter +
                " ORDER BY m.title " + ordering +
                " LIMIT " + moviesPerPage + " OFFSET " + ((page - 1) * moviesPerPage) + ";", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                movies.Add(MapToMovie(reader));
            }

            return movies;
        }

        public async Task<int> GetCountOfAllMoviesAsync()
        {
            //example of logging in repository layer
            _logger.LogInformation("Get all available movies - repository layer.");

            int count = 0;

            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT count(*) FROM movies", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                count = reader.GetInt32(0);
            }

            return count;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesWithDirectorsAndGenres(
            int releasedYearFilter,
            string ordering,
            int moviesPerPage,
            int page,
            string genre,
            string nameOfMovie)
        {
            var movies = new Dictionary<Guid, Movie>();

            using var conn = CreateConnection();
            await conn.OpenAsync();

            NpgsqlCommand cmd;

            if (!(genre == "nothing"))
            {
                cmd = new NpgsqlCommand($@"
                WITH paged_movies AS (
                SELECT *
                FROM movies
                WHERE release_year >= @year
                ORDER BY release_year {ordering}
                )
                SELECT
                DISTINCT m.id AS movie_id,
                m.title AS movie_title,
                m.release_year,
                m.duration_minutes,
                m.director_id,
                m.description,
                d.id AS director_id,
                d.name AS director_name,
                d.birthdate AS director_birthdate,
                d.nationality AS director_nationality,
                g.id AS genre_id,
                g.name AS genre_name
                FROM paged_movies m
                JOIN directors d ON m.director_id = d.id
                JOIN movie_genres mg ON m.id = mg.movie_id
                JOIN genres g ON mg.genre_id = g.id
                WHERE @genre IS NULL OR g.name ILIKE @genre
                ORDER BY m.release_year {ordering};", conn);

                cmd.Parameters.AddWithValue("@year", releasedYearFilter);
                cmd.Parameters.AddWithValue("@ordering", ordering);
                cmd.Parameters.AddWithValue("@limit", moviesPerPage);
                cmd.Parameters.AddWithValue("@genre", string.IsNullOrWhiteSpace(genre) ? DBNull.Value : genre);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * moviesPerPage);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var movieId = reader.GetGuid(reader.GetOrdinal("movie_id"));

                    if (!movies.TryGetValue(movieId, out var movie))
                    {
                        movie = new Movie
                        {
                            Id = movieId,
                            Title = reader.GetString(reader.GetOrdinal("movie_title")),
                            ReleaseYear = reader.GetInt32(reader.GetOrdinal("release_year")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            DirectorId = reader.GetGuid(reader.GetOrdinal("director_id")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Director = new Director
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("director_id")),
                                Name = reader.GetString(reader.GetOrdinal("director_name")),
                                Birthdate = reader.IsDBNull(reader.GetOrdinal("director_birthdate")) ? null : reader.GetDateTime(reader.GetOrdinal("director_birthdate")),
                                Nationality = reader.IsDBNull(reader.GetOrdinal("director_nationality")) ? null : reader.GetString(reader.GetOrdinal("director_nationality"))
                            },
                            Genres = new List<Genre>()
                        };

                        movies.Add(movieId, movie);
                    }

                    var genreId = reader.GetGuid(reader.GetOrdinal("genre_id"));
                    if (!movie.Genres.Any(g => g.Id == genreId))
                    {
                        movie.Genres.Add(new Genre
                        {
                            Id = genreId,
                            Name = reader.GetString(reader.GetOrdinal("genre_name"))
                        });
                    }
                }
            }
            else if (!(nameOfMovie == "nothing"))
            {
                cmd = new NpgsqlCommand($@"
                WITH paged_movies AS (
                SELECT *
                FROM movies
                WHERE release_year >= @year
                ORDER BY release_year {ordering}
                )
                SELECT
                DISTINCT m.id AS movie_id,
                m.title AS movie_title,
                m.release_year,
                m.duration_minutes,
                m.director_id,
                m.description,
                d.id AS director_id,
                d.name AS director_name,
                d.birthdate AS director_birthdate,
                d.nationality AS director_nationality,
                g.id AS genre_id,
                g.name AS genre_name
                FROM paged_movies m
                JOIN directors d ON m.director_id = d.id
                JOIN movie_genres mg ON m.id = mg.movie_id
                JOIN genres g ON mg.genre_id = g.id
                WHERE @name IS NULL OR m.title ILIKE @name
                ORDER BY m.release_year {ordering};", conn);

                cmd.Parameters.AddWithValue("@year", releasedYearFilter);
                cmd.Parameters.AddWithValue("@ordering", ordering);
                cmd.Parameters.AddWithValue("@limit", moviesPerPage);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * moviesPerPage);
                cmd.Parameters.AddWithValue("@name", string.IsNullOrWhiteSpace(nameOfMovie) ? DBNull.Value : $"%{nameOfMovie}%");

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var movieId = reader.GetGuid(reader.GetOrdinal("movie_id"));

                    if (!movies.TryGetValue(movieId, out var movie))
                    {
                        movie = new Movie
                        {
                            Id = movieId,
                            Title = reader.GetString(reader.GetOrdinal("movie_title")),
                            ReleaseYear = reader.GetInt32(reader.GetOrdinal("release_year")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            DirectorId = reader.GetGuid(reader.GetOrdinal("director_id")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Director = new Director
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("director_id")),
                                Name = reader.GetString(reader.GetOrdinal("director_name")),
                                Birthdate = reader.IsDBNull(reader.GetOrdinal("director_birthdate")) ? null : reader.GetDateTime(reader.GetOrdinal("director_birthdate")),
                                Nationality = reader.IsDBNull(reader.GetOrdinal("director_nationality")) ? null : reader.GetString(reader.GetOrdinal("director_nationality"))
                            },
                            Genres = new List<Genre>()
                        };

                        movies.Add(movieId, movie);
                    }

                    var genreId = reader.GetGuid(reader.GetOrdinal("genre_id"));
                    if (!movie.Genres.Any(g => g.Id == genreId))
                    {
                        movie.Genres.Add(new Genre
                        {
                            Id = genreId,
                            Name = reader.GetString(reader.GetOrdinal("genre_name"))
                        });
                    }
                }
            } else
            {
                cmd = new NpgsqlCommand($@"
                WITH paged_movies AS (
                SELECT *
                FROM movies
                WHERE release_year >= @year
                ORDER BY release_year {ordering}
                )
                SELECT
                DISTINCT m.id AS movie_id,
                m.title AS movie_title,
                m.release_year,
                m.duration_minutes,
                m.director_id,
                m.description,
                d.id AS director_id,
                d.name AS director_name,
                d.birthdate AS director_birthdate,
                d.nationality AS director_nationality,
                g.id AS genre_id,
                g.name AS genre_name
                FROM paged_movies m
                JOIN directors d ON m.director_id = d.id
                JOIN movie_genres mg ON m.id = mg.movie_id
                JOIN genres g ON mg.genre_id = g.id
                ORDER BY m.release_year {ordering};", conn);

                cmd.Parameters.AddWithValue("@year", releasedYearFilter);
                cmd.Parameters.AddWithValue("@ordering", ordering);
                cmd.Parameters.AddWithValue("@limit", moviesPerPage);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * moviesPerPage);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var movieId = reader.GetGuid(reader.GetOrdinal("movie_id"));

                    if (!movies.TryGetValue(movieId, out var movie))
                    {
                        movie = new Movie
                        {
                            Id = movieId,
                            Title = reader.GetString(reader.GetOrdinal("movie_title")),
                            ReleaseYear = reader.GetInt32(reader.GetOrdinal("release_year")),
                            DurationMinutes = reader.GetInt32(reader.GetOrdinal("duration_minutes")),
                            DirectorId = reader.GetGuid(reader.GetOrdinal("director_id")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Director = new Director
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("director_id")),
                                Name = reader.GetString(reader.GetOrdinal("director_name")),
                                Birthdate = reader.IsDBNull(reader.GetOrdinal("director_birthdate")) ? null : reader.GetDateTime(reader.GetOrdinal("director_birthdate")),
                                Nationality = reader.IsDBNull(reader.GetOrdinal("director_nationality")) ? null : reader.GetString(reader.GetOrdinal("director_nationality"))
                            },
                            Genres = new List<Genre>()
                        };

                        movies.Add(movieId, movie);
                    }

                    var genreId = reader.GetGuid(reader.GetOrdinal("genre_id"));
                    if (!movie.Genres.Any(g => g.Id == genreId))
                    {
                        movie.Genres.Add(new Genre
                        {
                            Id = genreId,
                            Name = reader.GetString(reader.GetOrdinal("genre_name"))
                        });
                    }
                }
            }

            return movies.Values.Skip((page - 1) * moviesPerPage).Take(moviesPerPage).ToList();
        }

        /// This method counts the number of movies based on the provided filters.
        public async Task<int> GetMoviesCountWithFilters(
            int releasedYearFilter,
            string genre,
            string nameOfMovie)
        {
            NpgsqlCommand cmd;

            using var conn = CreateConnection();
            await conn.OpenAsync();
            if (!(genre == "nothing"))
            {


                cmd = new NpgsqlCommand(@"
                SELECT COUNT(DISTINCT m.id)
                FROM movies m
                JOIN movie_genres mg ON m.id = mg.movie_id
                JOIN genres g ON mg.genre_id = g.id
                WHERE m.release_year >= @year
                  AND (@genre IS NULL OR g.name ILIKE @genre);", conn);

                cmd.Parameters.AddWithValue("@year", releasedYearFilter);
                cmd.Parameters.AddWithValue("@genre", string.IsNullOrWhiteSpace(genre) ? DBNull.Value : genre);
                cmd.Parameters.AddWithValue("@name", string.IsNullOrWhiteSpace(nameOfMovie) ? DBNull.Value : $"%{nameOfMovie}%");
            }
            else if (!(nameOfMovie == "nothing"))
            {


                cmd = new NpgsqlCommand(@"
                SELECT COUNT(DISTINCT m.id)
                FROM movies m
                JOIN movie_genres mg ON m.id = mg.movie_id
                JOIN genres g ON mg.genre_id = g.id
                WHERE m.release_year >= @year
                  AND (@name IS NULL OR m.title ILIKE @name);", conn);

                cmd.Parameters.AddWithValue("@year", releasedYearFilter);
                cmd.Parameters.AddWithValue("@genre", string.IsNullOrWhiteSpace(genre) ? DBNull.Value : genre);
                cmd.Parameters.AddWithValue("@name", string.IsNullOrWhiteSpace(nameOfMovie) ? DBNull.Value : $"%{nameOfMovie}%");
            }
            else
            {


                cmd = new NpgsqlCommand(@"
                SELECT COUNT(DISTINCT m.id)
                FROM movies m
                JOIN movie_genres mg ON m.id = mg.movie_id
                JOIN genres g ON mg.genre_id = g.id
                WHERE m.release_year >= @year;", conn);

                cmd.Parameters.AddWithValue("@year", releasedYearFilter);
                cmd.Parameters.AddWithValue("@genre", string.IsNullOrWhiteSpace(genre) ? DBNull.Value : genre);
                cmd.Parameters.AddWithValue("@name", string.IsNullOrWhiteSpace(nameOfMovie) ? DBNull.Value : $"%{nameOfMovie}%");
            }



            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task AddMovieWithGenresAsync(Movie movie)
        {
            var movieId = Guid.NewGuid();

            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var tx = await conn.BeginTransactionAsync();

            try
            {
                // Insert movie
                var movieCmd = new NpgsqlCommand(@"
            INSERT INTO movies (id, title, release_year, duration_minutes, director_id, description)
            VALUES (@id, @title, @year, @duration, @director, @desc)", conn);
                movieCmd.Parameters.AddWithValue("@id", movieId);
                movieCmd.Parameters.AddWithValue("@title", movie.Title);
                movieCmd.Parameters.AddWithValue("@year", movie.ReleaseYear);
                movieCmd.Parameters.AddWithValue("@duration", movie.DurationMinutes);
                movieCmd.Parameters.AddWithValue("@director", movie.DirectorId);
                movieCmd.Parameters.AddWithValue("@desc", (object?)movie.Description ?? DBNull.Value);
                await movieCmd.ExecuteNonQueryAsync();

                // Insert movie_genres
                foreach (var genre in movie.Genres)
                {
                    var genreCmd = new NpgsqlCommand(@"
                INSERT INTO movie_genres (movie_id, genre_id)
                VALUES (@movieId, @genreId)", conn);
                    genreCmd.Parameters.AddWithValue("@movieId", movieId);
                    genreCmd.Parameters.AddWithValue("@genreId", genre.Id);
                    await genreCmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
            }
        }
    }
}
