using System;

namespace MoviesWebApp.Model
{
    public class Actor
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
        public string? Nationality { get; set; }
        public List<Movie>? Movies { get; set; }
    }
}