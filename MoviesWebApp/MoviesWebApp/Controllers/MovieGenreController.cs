using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service;
using MoviesWebApp.Service.Common;
using System.Threading.Tasks;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieGenreController : ControllerBase
    {
        private readonly IMovieGenreService _movieGenreService;

        public MovieGenreController(IMovieGenreService movieGenreService)
        {
            _movieGenreService = movieGenreService;
        }
        [HttpGet]
        [Route("get-movie-genres/{movieId}")]
        public async Task<IActionResult> GetMovieGenresAsync(Guid movieId)
        {
            var genreIds = await _movieGenreService.GetMovieGenreIdsAsync(movieId);
            return Ok(genreIds);
        }
        [HttpPost]
        [Route("add-movie-genre")]
        public async Task<IActionResult> AddMovieGenreAsync([FromBody] MovieGenreREST movieGenreRest)
        {
            if (movieGenreRest == null || movieGenreRest.MovieId == Guid.Empty || movieGenreRest.GenreId == Guid.Empty)
            {
                return BadRequest("Invalid movie genre data.");
            }
            await _movieGenreService.AddMovieGenreAsync(movieGenreRest.MovieId, movieGenreRest.GenreId);
            return Ok("Movie genre added successfully.");
        }

        [HttpPut]
        [Route("update-movie-genre/{movieId}/{genreId}")]
        public async Task<IActionResult> UpdateMovieGenreAsync(Guid movieId, Guid genreId)
        {
            if (movieId == Guid.Empty || genreId == Guid.Empty)
            {
                return BadRequest("Invalid movie or genre ID.");
            }
            
            await _movieGenreService.AddMovieGenreAsync(movieId, genreId);
            return Ok("Movie genre updated successfully.");
        }
        [HttpDelete]
        [Route("delete-movie-genre/{movieId}/{genreId}")]
        public async Task<IActionResult> DeleteMovieGenreAsync(Guid movieId, Guid genreId)
        {
            if (movieId == Guid.Empty || genreId == Guid.Empty)
            {
                return BadRequest("Invalid movie or genre ID.");
            }
            await _movieGenreService.DeleteMovieGenreAsync(movieId, genreId);
            return Ok("Movie genre deleted successfully.");
        }
        
        [HttpDelete]
        [Route("clear-movie-genres/{movieId}")]
        public async Task<IActionResult> ClearMovieGenresAsync(Guid movieId)
        {
            if (movieId == Guid.Empty)
            {
                return BadRequest("Invalid movie ID.");
            }
            await _movieGenreService.ClearMovieGenresAsync(movieId);
            return Ok("All genres cleared for the movie.");
        }


    }
}
