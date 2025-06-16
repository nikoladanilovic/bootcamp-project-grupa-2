using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Model
{
    public class Director
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? Birthdate { get; set; }
        public string? Nationality { get; set; }
    }
}
