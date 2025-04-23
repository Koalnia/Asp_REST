using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Asp_REST.Entity
{
    public class User
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

        public string Name { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        [Column("verification_code")]
        public string VerificationCode { get; set; } = string.Empty;

        [Column("verification_expiration")]
        public DateTime? VerificationCodeExpiresAt { get; set; }

        public bool Enabled { get; set; }

        public User(string name, string phoneNumber, string email, string password)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            Password = password;
        }

        public User() { }

    }
}
   
