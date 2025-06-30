using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;
using System.IO;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService service, IMapper mapper, ILogger<MovieController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Get all available movies - controller layer.");
            var moviesREST = new List<MovieREST>();
            var movies = await _service.GetAllAsync();
            foreach (var movie in movies)
            {
                moviesREST.Add(_mapper.Map<MovieREST>(movie));
            }
            return Ok(moviesREST);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var movie = await _service.GetByIdAsync(id);
            if (movie == null) return NotFound();
            return Ok(_mapper.Map<MovieREST>(movie));
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieREST movieREST)
        {
            //await _service.AddAsync(movie);
            //return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);

            var movieDomain = _mapper.Map<Movie>(movieREST);

            try
            {
                await _service.AddAsync(movieDomain);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error creating movie: {ex.Message}");
            }
            // TODO: Save to database
            

            // Return mapped DTO from entity
            var movieResult = _mapper.Map<MovieREST>(movieDomain);
            return Ok(movieResult);
        }

        [HttpPost("with-genres")]
        public async Task<IActionResult> CreateWithGenres(MovieREST movieREST)
        {
            //await _service.AddAsync(movie);
            //return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);

            var movieDomain = _mapper.Map<Movie>(movieREST);

            try
            {
                _logger.LogInformation(movieDomain.Genres[0].Name);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                _logger.LogInformation("nema zanrova");
            }


            try
            {
                await _service.AddMovieWithGenresAsync(movieDomain);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error creating movie: {ex.Message}");
            }
            // TODO: Save to database


            // Return mapped DTO from entity
            var movieResult = _mapper.Map<MovieREST>(movieDomain);
            return Ok(movieResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MovieREST movieREST)
        {
            if (id != movieREST.Id) return BadRequest("ID mismatch");
            try
            {
                await _service.UpdateAsync(_mapper.Map<Movie>(movieREST));
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error updating movie: {ex.Message}");
            }
            
            return await GetAll();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var movie = await _service.GetByIdAsync(id);
            if (movie == null) return NotFound();
            await _service.DeleteAsync(id);
            return await GetAll();
        }
        [HttpGet("genres-{id}")]
        public async Task<IActionResult> GetGenresOfMovie(Guid id)
        {
            Movie? movie = null;
            try
            {
                movie = await _service.GetGenresOfMovieAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error retrieving genres: {ex.Message}");
            }
            
            List<GenreREST> genresREST = new List<GenreREST>();
            
            foreach (var genre in movie.Genres)
            {
                genresREST.Add(_mapper.Map<GenreREST>(genre));
            }
            return Ok(genresREST);
        }

        [HttpGet("reviews-{id}")]
        public async Task<IActionResult> GetReviewsOfMovie(Guid id)
        {
            Movie? movie = null;
            try
            {
                movie = await _service.GetReviewsOfMovieAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error retrieving reviews: {ex.Message}");
            }
            
            List<ReviewREST> reviewsREST = new List<ReviewREST>();
            
            foreach (var review in movie.Reviews)
            {
                reviewsREST.Add(_mapper.Map<ReviewREST>(review));
            }
            return Ok(reviewsREST);
        }

        [HttpGet("curated")]
        public async Task<IActionResult> GetAllMoviesCurated(
            int releasedYearFilter = 1900,
            string ordering = "ASC",
            int moviesPerPage = 5,
            int page = 1)
        {
            if (moviesPerPage <= 0 || page <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }
            try
            {
                var movies = await _service.GetAllMoviesCuratedAsync(releasedYearFilter, ordering, moviesPerPage, page);
                var moviesREST = _mapper.Map<IEnumerable<MovieREST>>(movies);
                return Ok(moviesREST);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error retrieving curated movies: {ex.Message}");
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCountOfAllMovies(int releasedYearFilter = 1900, string genre = "nothing", string nameOfMovie = "nothing")
        {
            try
            {
                int count = await _service.GetMoviesCountWithFilters(releasedYearFilter, genre, nameOfMovie);
                return Ok(count);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error retrieving movie count: {ex.Message}");
            }
        }

        [HttpGet("with-directors-and-genres")]
        public async Task<IActionResult> GetAllMoviesWithDirectorsAndGenres(
            int releasedYearFilter = 1900,
            string ordering = "ASC",
            int moviesPerPage = 5,
            int page = 1,
            string genre = "nothing",
            string nameOfMovie = "nothing")
        {
            if (moviesPerPage <= 0 || page <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }
            try
            {
                var movies = await _service.GetAllMoviesWithDirectorsAndGenres(releasedYearFilter, ordering, moviesPerPage, page, genre, nameOfMovie);
                var moviesREST = _mapper.Map<IEnumerable<MovieREST>>(movies);
                return Ok(moviesREST);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return BadRequest($"Error retrieving movies with directors and genres: {ex.Message}");
            }
        }
    }
}
