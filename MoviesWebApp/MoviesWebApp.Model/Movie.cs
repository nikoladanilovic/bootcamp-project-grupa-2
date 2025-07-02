using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Model
{
    public class Movie
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
        public List<Actor>? Actors { get; set; } = new();

    }
}
