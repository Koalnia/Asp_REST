using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp_REST.Services;
using Asp_REST.Dto;
using Asp_REST.Data;
using Asp_REST.Exceptions;

namespace Asp_REST.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController(UserService userService, AppDbContext context) : ControllerBase
    {
        [Authorize]
        [HttpGet("me")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value; // Pobranie emaila użytkownika z tokenu
            return Ok(new { Email = userEmail });

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are and admin!");
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserShowDto>> GetAllUsers()
        {
            var users = userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<UserShowDto> GetUserById(long id)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id)
                ?? throw new UserException("Brak użytkownika o id: " + id);
            UserShowDto userShowDto = new(user);
            return Ok(userShowDto);
        }

        [HttpGet("code/{id}")]
        public ActionResult<string> GetUserVerificationCode(long id)
        {
            string code = userService.GetUserVerificationCode(id);
            return Ok($"Kod weryfikacyjny użytkownika to: {code}");
        }

        [HttpPatch("{id}")]
        public ActionResult<UserShowDto> EditUser(long id, [FromBody] EditUserDto editUserDto)
        {
            var userShowDto = userService.EditUser(editUserDto, id);
            return Ok(userShowDto);
        }

        [HttpDelete("{id}")]
        public ActionResult<string> DeleteUser(long id)
        {
            var result = userService.DeleteUser(id);
            return Ok(result);
        }
    }
}
