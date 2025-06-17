using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Model
{
    
        public class MovieGenre
        {
            [Key]
            [Column(Order = 1)]
            public Guid MovieId { get; set; }

            [Key]
            [Column(Order = 2)]
            public Guid GenreId { get; set; }

            
            [ForeignKey("MovieId")]
            public virtual Movie Movie { get; set; }

            [ForeignKey("GenreId")]
            public virtual Genre Genre { get; set; }
        }
    
}
