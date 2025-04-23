using Asp_REST.Dto;
using Asp_REST.Entity;
using Asp_REST.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Asp_REST.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Asp_REST.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration, EmailService emailService)
    {

        public User Login(LoginUserDto loginUserDto)
        {
            var user =  context.Users.FirstOrDefault(u => u.Email == loginUserDto.Email);
            if (user is null)
            {
                throw new UserException("Brak użytkownika o takim adresie email");
            }
            if (user.Enabled == false)
            {
                throw new UserException("Konto nie zostało jeszcze zweryfikowane.Potwierdź swój adres email");
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, loginUserDto.Password)
                == PasswordVerificationResult.Failed)
            {
                throw new UserException("Podane hasło nie jest poprawne");
            }

            return user;
        }

        public User SignUp(RegisterUserDto registerUserDto )
        {
            if (context.Users.Any(u => u.Email == registerUserDto.Email))
            {
                throw new UserException("Konto o takim adresie email już istnieje");
            }

            var user = new User(
                registerUserDto.Name,
                registerUserDto.PhoneNumber,
                registerUserDto.Email,
                new PasswordHasher<User>().HashPassword(null, registerUserDto.Password)
             );
            user.Role = Role.User.ToString();
            user.Enabled = false;
            user.VerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(15);
            var random = new Random();
            user.VerificationCode = random.Next(100000, 1000000).ToString();

            context.Users.Add(user);
            context.SaveChanges();

            SendVerificationEmail(user);

            return user;
        }

        public LoginResponse CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role.ToString(), user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return new LoginResponse(jwtToken, "1h");
        }

        private async Task SendVerificationEmail(User user)
        {
            string subject = "Weryfikacja konta";
            string verificationCode = "Kod weryfikacyjny " + user.VerificationCode;
            string htmlMessage = "<html>"
                    + "<body style=\"font-family: Arial, sans-serif;\">"
                    + "<div style=\"background-color: #f5f5f5; padding: 20px;\">"
                    + "<h2 style=\"color: #333;\">Witam w aplikacji testowej Asp.Net Core Api!</h2>"
                    + "<p style=\"font-size: 16px;\">Użyj tego kodu do weryfikacji</p>"
                    + "<div style=\"background-color: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0,0,0,0.1);\">"
                    + "<h3 style=\"color: #333;\">Verification Code:</h3>"
                    + "<p style=\"font-size: 18px; font-weight: bold; color: #007bff;\">" + verificationCode + "</p>"
                    + "</div>"
                    + "</div>"
                    + "</body>"
                    + "</html>";

              await emailService.SendEmail(user.Email, subject, htmlMessage);
           
            
        }

        public void ResendVerificationEmail(String email)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user is null)
            {
                throw new UserException("Brak takiego użytkownika");
            }
            if (user.Enabled == true)
                throw new UserException("Użytkownik został już zweryfikowany");
            user.VerificationCodeExpiresAt = DateTime.UtcNow.AddHours(1);
            Random random = new Random();
            user.VerificationCode = random.Next(100000, 1000000).ToString();

            context.SaveChanges();

            SendVerificationEmail(user);
        }

        public void VerifyUser(VerifyUserDto verifyUserDto)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == verifyUserDto.Email);
            if (user is null)
                throw new UserException("Brak takiego użytkownika");
            if (user.Enabled == true)
                throw new UserException("Użytkownik został już zweryfikowany");
            if (user.VerificationCodeExpiresAt < DateTime.UtcNow)
                throw new UserException("Kod weryfikacyjny stracił swoją ważność");
            if (user.VerificationCode.Equals(verifyUserDto.VerificationCode))
            {
                user.Enabled = true;
                user.VerificationCode = "";
                user.VerificationCodeExpiresAt = DateTime.UtcNow;
                context.SaveChanges();
            }
            else
                throw new UserException("Niepoprawny kod weryfikacyjny");
        }
}
}

