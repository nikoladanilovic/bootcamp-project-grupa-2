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

        public MovieREST(Movie movie)
        {
            Id = movie.Id;
            Title = movie.Title;
            ReleaseYear = movie.ReleaseYear;
            DurationMinutes = movie.DurationMinutes;
            DirectorId = movie.DirectorId;
            Description = movie.Description;
            
        }
    }
}
