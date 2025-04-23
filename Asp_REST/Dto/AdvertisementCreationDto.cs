using Asp_REST.Entity;
using System.ComponentModel.DataAnnotations;

namespace Asp_REST.Dto
{
    public class AdvertisementCreationDto
    {
        [Required(ErrorMessage = "Tytuł ogłoszenia nie może być pusty")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Tytuł ogłoszenia musi zawierać od 8 do 50 znaków")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Opis ogłoszenia nie może być pusty")]
        [StringLength(100, MinimumLength = 9, ErrorMessage = "Opis ogłoszenia musi zawierać od 9 do 100 znaków")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cena ogłoszenia nie może być pusta")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Cena ogłoszenia musi zawierać od 1 do 50 znaków")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Czas trwania ogłoszenia nie może być pusty")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Czas trwania ogłoszenia musi zawierać od 4 do 50 znaków")]
        public string Duration { get; set; }
    }
}
