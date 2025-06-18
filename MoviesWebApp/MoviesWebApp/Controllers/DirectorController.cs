using AutoMapper;
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
    public class DirectorController : ControllerBase
    {
        private readonly IDirectorService _service;

        private readonly IMapper _mapper;


        // Constructor injection for the director service
        public DirectorController(IDirectorService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var directors = await _service.GetAllAsync();
            List<DirectorREST> directorsREST = new List<DirectorREST>();
            foreach (var director in directors)
            {
                directorsREST.Add(_mapper.Map<DirectorREST>(director));
            }
            return Ok(directorsREST);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var director = await _service.GetByIdAsync(id);
            DirectorREST directorREST = _mapper.Map<DirectorREST>(director);
            if (director == null) return NotFound();
            return Ok(directorREST);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DirectorREST directorREST)
        {
            //await _service.AddAsync(director);
            //return CreatedAtAction(nameof(GetById), new { id = director.Id }, director);
            
            var directorDomain = _mapper.Map<Director>(directorREST);

            // TODO: Save to database
            await _service.AddAsync(directorDomain);

            // Return mapped DTO from entity
            var directorResult = _mapper.Map<DirectorREST>(directorDomain);
            return Ok(directorResult);
            

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DirectorREST directorREST)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            directorREST.Id = id; // Ensure ID is correct
            var directorDomain = _mapper.Map<Director>(directorREST);
            await _service.UpdateAsync(directorDomain);
            return await GetAll();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id);
            return await GetAll();
        }
    }
}
