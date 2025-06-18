using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly string _connectionString;

        public ReviewRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            var reviews = new List<Review>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"reviews\"", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reviews.Add(new Review
                        {
                            Id = reader.GetGuid(0),
                            UserId = reader.GetGuid(1),
                            MovieId = reader.GetGuid(2),
                            Rating = reader.GetInt32(3),
                            Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }
            return reviews;
        }

        public async Task<Review?> GetReviewByIdAsync(Guid id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"reviews\" WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Review
                            {
                                Id = reader.GetGuid(0),
                                UserId = reader.GetGuid(1),
                                MovieId = reader.GetGuid(2),
                                Rating = reader.GetInt32(3),
                                Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task CreateReviewAsync(List<Review> reviews)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO \"reviews\" (\"id\", \"user_id\", \"movie_id\", \"rating\", \"comment\", \"created_at\") VALUES (@Id, @UserId, @MovieId, @Rating, @Comment, @CreatedAt)", conn))
                {
                    foreach (var review in reviews)
                    {
                        review.Id = Guid.NewGuid();
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Id", review.Id);
                        cmd.Parameters.AddWithValue("@UserId", review.UserId);
                        cmd.Parameters.AddWithValue("@MovieId", review.MovieId);
                        cmd.Parameters.AddWithValue("@Rating", review.Rating);
                        cmd.Parameters.AddWithValue("@Comment", review.Comment ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task<bool> UpdateReviewAsync(Guid id, Review review)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE \"reviews\" SET \"user_id\" = @UserId, \"movie_id\" = @MovieId, \"rating\" = @Rating, \"comment\" = @Comment WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@UserId", review.UserId);
                    cmd.Parameters.AddWithValue("@MovieId", review.MovieId);
                    cmd.Parameters.AddWithValue("@Rating", review.Rating);
                    cmd.Parameters.AddWithValue("@Comment", review.Comment ?? (object)DBNull.Value);
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        public async Task DeleteReviewAsync(Guid id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM \"reviews\" WHERE \"id\" = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            var reviews = new List<Review>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM \"reviews\" WHERE \"user_id\" = @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reviews.Add(new Review
                            {
                                Id = reader.GetGuid(0),
                                UserId = reader.GetGuid(1),
                                MovieId = reader.GetGuid(2),
                                Rating = reader.GetInt32(3),
                                Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return reviews;
        }
        public async Task<bool> ReviewExistsAsync(Guid userId, Guid movieId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(1) FROM \"reviews\" WHERE \"user_id\" = @UserId AND \"movie_id\" = @MovieId", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@MovieId", movieId);
                    var count = (long)await cmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

    }
}