using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using MoviesWebApp.Model;
using MoviesWebApp.Service.Common;
using MoviesWebApp.RESTModels;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly IMovieService _service;

        public MovieController(IMovieService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var moviesREST = new List<MovieREST>();
            var movies = await _service.GetAllAsync();
            foreach (var movie in movies)
            {
                moviesREST.Add(new MovieREST(movie));
            }
            return Ok(moviesREST);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var movie = await _service.GetByIdAsync(id);
            if (movie == null) return NotFound();
            return Ok(new MovieREST(movie));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie)
        {
            await _service.AddAsync(movie);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Movie movie)
        {
            if (id != movie.Id) return BadRequest("ID mismatch");
            await _service.UpdateAsync(movie);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
