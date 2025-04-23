using Asp_REST.Entity;

namespace Asp_REST.Dto
{
    public class AdvertisementShowDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Duration { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CreatedAt { get; set; }

        public AdvertisementShowDto(Advertisement advertisement)
        {
            Title = advertisement.Title;
            Description = advertisement.Description;
            Price = advertisement.Price;
            Duration = advertisement.Duration;
            Email = advertisement.User.Email; 
            PhoneNumber = advertisement.User.PhoneNumber;
            CreatedAt = advertisement.CreatedAt;
        }
    }
}
