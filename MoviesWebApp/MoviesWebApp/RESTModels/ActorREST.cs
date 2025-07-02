using System;

namespace MoviesWebApp.RESTModels
{
    public class ActorREST
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
        public string? Nationality { get; set; }
    }
}