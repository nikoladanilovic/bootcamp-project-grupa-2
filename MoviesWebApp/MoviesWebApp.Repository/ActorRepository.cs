using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository
{
    public class ActorRepository : IActorRepository
    {
        private readonly string _connectionString;

        public ActorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private NpgsqlConnection CreateConnection() => new(_connectionString);

        public async Task<IEnumerable<Actor>> GetAllAsync()
        {
            var actors = new List<Actor>();
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT id, name, birthdate, nationality FROM actors", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                actors.Add(new Actor
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Birthdate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                    Nationality = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return actors;
        }

        public async Task<Actor?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT id, name, birthdate, nationality FROM actors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Actor
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Birthdate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                    Nationality = reader.IsDBNull(3) ? null : reader.GetString(3)
                };
            }
            return null;
        }

        public async Task AddAsync(Actor actor)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(
                "INSERT INTO actors (id, name, birthdate, nationality) VALUES (@id, @name, @birthdate, @nationality)", conn);
            cmd.Parameters.AddWithValue("id", actor.Id);
            cmd.Parameters.AddWithValue("name", actor.Name);
            cmd.Parameters.AddWithValue("birthdate", (object?)actor.Birthdate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("nationality", (object?)actor.Nationality ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Actor actor)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand(
                "UPDATE actors SET name = @name, birthdate = @birthdate, nationality = @nationality WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", actor.Id);
            cmd.Parameters.AddWithValue("name", actor.Name);
            cmd.Parameters.AddWithValue("birthdate", (object?)actor.Birthdate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("nationality", (object?)actor.Nationality ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("DELETE FROM actors WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}