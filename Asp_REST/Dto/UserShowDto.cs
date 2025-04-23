using Asp_REST.Entity;

namespace Asp_REST.Dto
{
    public class UserShowDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public UserShowDto() { }

        public UserShowDto(string name, string phoneNumber, string email, string role)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            Role = role;
        }

        public UserShowDto(User user)
        {
            Name = user.Name;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            Role = user.Role.ToString(); 
        }
    }
}
