using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Model
{
   public class Review
    {
       public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }
        public int Rating { get; set; } 
        public string? Comment { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }
}
