using MoviesWebApp.Model;
using System.Collections.Generic;

namespace MoviesWebApp.RESTModels
{
    public class MovieREST
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public int DurationMinutes { get; set; }
        public Guid DirectorId { get; set; }
        public string? Description { get; set; }
        public Director? Director { get; set; } = null!;
        public List<Genre>? Genres { get; set; } = null!;
        public List<Review>? Reviews { get; set; } = null!;
        public List<ActorREST>? Actors { get; set; } = new();
    }
}
