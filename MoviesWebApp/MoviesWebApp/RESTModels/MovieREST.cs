using MoviesWebApp.Model;

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
    }
}
