using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;
using Npgsql;

namespace MoviesWebApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService usersService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService usersService, IMapper mapper)
        {
            this.usersService = usersService;
            _mapper = mapper;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await usersService.GetAllUsersAsync();
            var usersRest = _mapper.Map<IEnumerable<UserREST>>(users);
            return Ok(usersRest);
        }

        [HttpGet("get-users/{id:guid}")]
        public async Task<ActionResult<UserREST>> GetUsersAsync(Guid id)
        {
            var user = await usersService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var usersRest = _mapper.Map<UserREST>(user);
            return Ok(usersRest);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUserAsync([FromBody] List<UserREST> usersREST)
        {
            if (usersREST == null || usersREST.Count == 0)
                return BadRequest("Users cannot be null or empty.");

            foreach (var user in usersREST)
            {
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                    return BadRequest("Username, Email, and Password are required.");
            }

            try
            {
                var users = _mapper.Map<List<User>>(usersREST);
                await usersService.CreateUserAsync(users);
                return Ok(_mapper.Map<List<UserREST>>(users));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                return BadRequest($"Duplicate entry: Username or Email already exist");
            }
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UserREST userREST)
        {
            if (userREST == null)
                return BadRequest("User cannot be null.");

            if (string.IsNullOrEmpty(userREST.Username) || string.IsNullOrEmpty(userREST.Email))
                return BadRequest("Username and Email are required.");

            try
            {
                var user = _mapper.Map<User>(userREST);
                bool updated = await usersService.UpdateUserAsync(id, user);
                if (!updated)
                    return NotFound("User not found or not updated.");

                return Ok(_mapper.Map<UserREST>(user));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                return BadRequest($"Duplicate entry:email or use already exist");
            }
        }

        [HttpDelete("delete-user/{id:guid}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            var user = await usersService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            await usersService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpGet("debug-users/{id:guid}")]
        public async Task<ActionResult<UserREST>> GetDebugUserAsync(Guid id)
        {
            var user = await usersService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRest = _mapper.Map<UserREST>(user);
            return Ok(userRest);
        }
    }
}