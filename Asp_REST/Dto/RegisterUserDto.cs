using System.ComponentModel.DataAnnotations;

namespace Asp_REST.Dto
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Imię nie może być puste")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Imię musi zawierać co najmniej 10 znaków")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Numer telefonu nie może być pusty")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Numer telefonu musi zawierać od 9 do 12 znaków")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "E-mail nie może być pusty")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "E-mail musi zawierać od 8 do 50 znaków")]
        [EmailAddress(ErrorMessage = "E-mail musi być poprawnym adresem e-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło nie może być puste")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$@!%&*?])[A-Za-z\\d#$@!%&*?]{8,20}$",
            ErrorMessage = "Hasło musi zawierać: 1 dużą literę, 1 małą, 1 liczbę, 1 znak specjalny oraz mieć od 8 do 16 znaków")]
        public string Password { get; set; }
    }
}
