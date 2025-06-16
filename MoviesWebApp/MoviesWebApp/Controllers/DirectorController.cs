using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorController : Controller
    {
        private readonly IDirectorService _service;

        // Constructor injection for the director service
        public DirectorController(IDirectorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var directors = await _service.GetAllAsync();
            List<DirectorREST> directorsREST = new List<DirectorREST>();
            foreach (var director in directors)
            {
                directorsREST.Add(new DirectorREST(director));
            }
            return Ok(directorsREST);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var director = await _service.GetByIdAsync(id);
            DirectorREST directorREST = new DirectorREST(director);
            if (director == null) return NotFound();
            return Ok(directorREST);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Director director)
        {
            await _service.AddAsync(director);
            return CreatedAtAction(nameof(GetById), new { id = director.Id }, director);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Director director)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            director.Id = id; // Ensure ID is correct
            await _service.UpdateAsync(director);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
