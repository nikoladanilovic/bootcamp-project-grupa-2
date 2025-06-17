namespace MoviesWebApp.RESTModels
{
    public class ReviewREST
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
