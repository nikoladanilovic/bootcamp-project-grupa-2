using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _service;
        private readonly IMapper _mapper;

        public ActorController(IActorService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorREST>>> GetAll()
        {
            var actors = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ActorREST>>(actors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActorREST>> GetById(Guid id)
        {
            var actor = await _service.GetByIdAsync(id);
            if (actor == null) return NotFound();
            return Ok(_mapper.Map<ActorREST>(actor));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ActorREST actorREST)
        {
            var actor = _mapper.Map<Actor>(actorREST);
            await _service.AddAsync(actor);
            return Ok(_mapper.Map<ActorREST>(actor));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ActorREST actorREST)
        {
            var actor = _mapper.Map<Actor>(actorREST);
            actor.Id = id;
            await _service.UpdateAsync(actor);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}