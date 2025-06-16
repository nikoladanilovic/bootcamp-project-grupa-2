using MoviesWebApp.Model;

namespace MoviesWebApp.RESTModels
{
    public class DirectorREST
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? Birthdate { get; set; }
        public string? Nationality { get; set; }
        public DirectorREST(Director director)
        {
            this.Id = director.Id;
            this.Name = director.Name;
            this.Birthdate = director.Birthdate;
            this.Nationality = director.Nationality;
        }
        
    }
}
