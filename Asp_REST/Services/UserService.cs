using System.Runtime.InteropServices;
using System.Security.Claims;
using Asp_REST.Data;
using Asp_REST.Dto;
using Asp_REST.Entity;
using Asp_REST.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Asp_REST.Services
{
    public class UserService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        public List<UserShowDto> GetAllUsers()
        {
            var users = context.Users.ToList();
            return users.Select(user => mapper.Map<UserShowDto>(user)).ToList();
        }

        public string GetUserVerificationCode(long userId)
        {
            var userEmail = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            var currentUser = context.Users.FirstOrDefault(u => u.Email == userEmail)
                ?? throw new UserException("Wystąpił problem z tokenem jwt");
            if (!currentUser.Role.Equals(Role.Admin.ToString()))
                throw new UserException("Tylko Admin może sprawdzić kod weryfikacyjny");
            if (currentUser.Id.Equals(userId))
                throw new UserException("Nie możesz dostać kodu weryfikacyjnego swojego konta, sprawdź swój adres email");
            var user = context.Users.FirstOrDefault(u => u.Id == userId)
                ?? throw new UserException($"Brak użytkownika o id: {userId}");
            if (user.Enabled == true)
                throw new UserException("Konto jest już zweryfikowane");
            return user.VerificationCode;
        }

        public User CheckUserAccess(long userId, bool toDelete)
        {
            var userEmail = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            var currentUser = context.Users.FirstOrDefault(u => u.Email == userEmail)
                ?? throw new UserException("Wystąpił problem z tokenem jwt");
            if (currentUser.Id != userId && currentUser.Role != Role.Admin.ToString())
            {
                throw new UserException("Tylko Admin może edytować lub usuwać nie swoje konto");
            }

            if (currentUser.Id == userId && toDelete)
            {
                throw new UserException("Nie możesz usunąć swojego konta, skontaktuj się z Adminem");
            }

            return context.Users.FirstOrDefault(u => u.Id == userId)
                ?? throw new UserException($"Brak użytkownika o id: {userId}");
        }



        public UserShowDto EditUser(EditUserDto editUserDto, long id)
        {
            User user = CheckUserAccess(id, false);
            user.Name = editUserDto.Name;
            user.PhoneNumber = editUserDto.PhoneNumber;
            user.Password = new PasswordHasher<User>().HashPassword(null, editUserDto.Password);

            context.Users.Update(user);
            context.SaveChanges();

            return new UserShowDto(user);
        }

        public string DeleteUser(long id)
        {
            User user = CheckUserAccess(id, true);
            context.Remove(user);
            context.SaveChanges();

            return "Usunięto użytkownika: " + user.Name;
        }
    }
}
