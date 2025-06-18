using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MoviesWebApp.Model;
using MoviesWebApp.Service.Common;
using MoviesWebApp.RESTModels;
using AutoMapper;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly AutoMapper.IMapper _mapper;

        public GenreController(IGenreService genreService, IMapper mapper)
        {
            _genreService = genreService;
            _mapper = mapper;
        }

        [HttpGet("get-genres")]
        public async Task<IEnumerable<GenreREST>> GetgenresAsync()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return _mapper.Map<IEnumerable<GenreREST>>(genres);
        }

        [HttpPost("add-genre")]
        public async Task<IActionResult> AddGenreAsync([FromBody] List<GenreREST> genresRest)
        {
            if (genresRest == null || !genresRest.Any())
            {
                return BadRequest("Invalid genre data.");
            }

            var genres = _mapper.Map<List<Genre>>(genresRest);
            foreach (var genre in genres)
            {
                genre.Id = Guid.NewGuid(); 
            }
            await _genreService.AddGenreAsync(genres);
            return Ok("genre added successfully.");
        }

        [HttpPut("update-genre/{id}")]
        public async Task<IActionResult> UpdategenreAsync(Guid id, [FromBody] GenreREST genreRest)
        {
            if (genreRest == null)
            {
                return BadRequest("Invalid genre data.");
            }

            var genre = _mapper.Map<Genre>(genreRest);
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
            var genreRest = _mapper.Map<GenreREST>(genre);
            return Ok(genreRest.Name);
        }
        [HttpGet("get-movie-genres-by-genre-id/{genreId}")]
        public async Task<IActionResult> GetMovieGenresByGenreIdAsync(Guid genreId)
        {
            if (genreId == Guid.Empty)
            {
                return BadRequest("Invalid genre ID.");
            }
            var genre = await _genreService.GetMovieGenresByGenreIdAsync(genreId);

            return Ok(genre);
        }
    }
}
