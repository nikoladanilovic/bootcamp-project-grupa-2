using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using MoviesWebApp.Model;
using MoviesWebApp.Repository;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;


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
            try
            {
                await usersService.CreateUserAsync(users);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating users: {ex.Message}");
            }
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Email and password are required.");

            var user = await usersService.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid email or password.");

            return Ok(new UserREST
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            });
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


        // Login, register, and other user-related endpoints can be added here


        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] User req)
        {
            List<User> users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = req.Username,
                    Email = req.Email,
                    Password = req.Password,
                    CreatedAt = DateTime.UtcNow
                }
            };

            try
            {
                await usersService.CreateUserAsync(users);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating users: {ex.Message}");
            }

            string token;
            try
            {
                token = GenerateJwtToken(req.Username);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Token generation failed: {ex.Message}");
            }
            return Ok(new { token });

        }

        [HttpPost("login-user")]
        public IActionResult Login([FromBody] User req)
        {
            // Validate user credentials
            if (string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Password))
                return BadRequest("Username and password are required.");
            var user = usersService.GetUserByEmailAsync(req.Email).Result;
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
                return Unauthorized("Invalid username or password.");

            // Generate JWT token
            return Ok(new { token = GenerateJwtToken(user.Username) });
        }

        [Authorize]
        [HttpGet("protected-user")]
        public IActionResult Protected()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("You must be logged in to access this route.");

            // This is a protected route, you can access user claims here
            return Ok(new { message = $"Hello, {username}. You accessed a protected route!" });
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-very-long-secret-key-that-is-at-least-33-bytes"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }

}

