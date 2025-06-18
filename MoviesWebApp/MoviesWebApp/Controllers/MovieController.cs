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

        public MovieController(IMovieService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
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

            // TODO: Save to database
            await _service.AddAsync(movieDomain);

            // Return mapped DTO from entity
            var movieResult = _mapper.Map<DirectorREST>(movieDomain);
            return Ok(movieResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MovieREST movieREST)
        {
            if (id != movieREST.Id) return BadRequest("ID mismatch");
            await _service.UpdateAsync(_mapper.Map<Movie>(movieREST));
            return await GetAll();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return await GetAll();
        }
    }
}
