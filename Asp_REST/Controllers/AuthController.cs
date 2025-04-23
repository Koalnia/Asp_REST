using System.Security.Claims;
using Asp_REST.Dto;
using Asp_REST.Entity;
using Asp_REST.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp_REST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        [HttpPost("signup")]
        public ActionResult<UserShowDto> SignUp([FromBody] RegisterUserDto registerUserDto)
        {
           if (!ModelState.IsValid)
           {
                return BadRequest(ModelState);
           }
           var user =  authService.SignUp(registerUserDto);
           UserShowDto userShowDto = new (user);
           return Ok(userShowDto);
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = authService.Login(loginUserDto);

            return Ok(authService.CreateToken(user));
        }

        [HttpPost("verify")]
        public ActionResult<string> Verify([FromBody] VerifyUserDto verifyUserDto)
        {
            authService.VerifyUser(verifyUserDto);

            return Ok("Użytkownik został poprawnie zweryfikowany");
        }

        [HttpPost("resend")]
        public ActionResult<string> Resend(string email)
        {
            authService.ResendVerificationEmail(email);
            return Ok("Ponownie wysłano kod weryfikacyjny");
        }

    }
}
