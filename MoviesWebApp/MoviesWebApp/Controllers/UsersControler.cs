using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Model;
using MoviesWebApp.Repository;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;
using System.Numerics;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersControler : ControllerBase
    {
        public readonly IUsersService usersService;
        public UsersControler(IUsersService usersService)
        {
            this.usersService = usersService;
        }
        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await usersService.GetAllUsersAsync();
            var usersRest = users.Select(u => new UserREST
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Password = u.Password
            });

            return Ok(users);
        }
        [HttpGet("get-users/{id}")]
        public async Task<ActionResult<UserREST>> GetUsersAsync(Guid id)
        {
            var user = await usersService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var usersRest = new UserREST
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password
            };

            return Ok(usersRest);
        }
        [HttpPost("create-user")]
       public async Task <IActionResult>CreateUserAsync([FromBody] List<UserREST> usersREST)
        {
            if (usersREST == null || usersREST.Count == 0)
            {
                return BadRequest("Users cannot be null or empty.");
            }

            var users = new List<User>();
            foreach (var user in usersREST)
            {
                users.Add(new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password,
                });
            }

            await usersService.CreateUserAsync(users);
            return Ok();
        }
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UserREST userREST)
        {
            if (userREST == null)
                return BadRequest("User cannot be null.");

            var user = new User
            {
                Id = userREST.Id,
                Username = userREST.Username,
                Email = userREST.Email,
                Password = userREST.Password
            };

            bool updated = await usersService.UpdateUserAsync(id, user);
            if (!updated)
                return NotFound("User not found or not updated.");

            return Ok("User updated.");
        }
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeletePlayerAsync(Guid id)
        {
            await usersService.DeleteUserAsync(id);
            return NoContent();
        }

    }

}

