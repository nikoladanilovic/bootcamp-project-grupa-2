using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoviesWebApp.Model;
using MoviesWebApp.Service.Common;
using MoviesWebApp.RESTModels;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class genresController : Controller
    {
        private readonly IGenreService _genreService;

        public genresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet("get-genres")]
        public async Task<IEnumerable<GenreREST>> GetgenresAsync()
        {
            var genres = await _genreService.GetAllGenresAsync();
            var genresREST = new List<GenreREST>();

            foreach (var genre in genres)
            {
                genresREST.Add(new GenreREST
                {
                    Id = genre.Id,
                    Name = genre.Name ?? "Unknown",
                });
            }

            return genresREST;
        }

        [HttpPost("add-genre")]
        public async Task<IActionResult> AddGenreAsync([FromBody] List<GenreREST> genresRest)
        {
            var genres = new List<Genre>();

            if (genresRest == null)
            {
                return BadRequest("Invalid genre data.");
            }
            foreach (var genreRest in genresRest)
            {
                var genre = new Genre
                {
                    Id = genreRest.Id, // Fix: Generate a new Guid for Genre.Id
                    Name = genreRest.Name ?? "Unknown",
                };
                genres.Add(genre);
            }
            await _genreService.AddGenreAsync(genres);
            return Ok("genre added successfully.");
        }

        [HttpPut("update-genre/{id}")]
        public async Task<IActionResult> UpdategenreAsync(Guid id, [FromBody] GenreREST genreRest)
        {
            var genre = new Genre
            {
                Id = genreRest.Id,
                Name = genreRest.Name ?? "Unknown",
            };

            await _genreService.UpdateGenreAsync(id, genre);

            return Ok("genre updated successfully.");
        }

        [HttpDelete("delete-genre/{id}")]
        public async Task<IActionResult> DeletegenreAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid genre ID.");
            }
            await _genreService.DeleteGenreAsync(id);
            return Ok("genre deleted successfully.");
        }
        [HttpGet("get-genre-by-ID/{id}")]
        public async Task<IActionResult> GetGenreByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid genre ID.");
            }
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound("Genre not found.");
            }
            var genreRest = new GenreREST
            {
                Id = genre.Id,
                Name = genre.Name ?? "Unknown",
            };
            return Ok(genreRest.Name);
        }
    }
}

