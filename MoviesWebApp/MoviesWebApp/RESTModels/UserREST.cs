﻿namespace MoviesWebApp.RESTModels
{
    public class UserREST
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
    }
}
