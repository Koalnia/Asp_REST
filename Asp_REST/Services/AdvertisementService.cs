using System.Security.Claims;
using Asp_REST.Data;
using Asp_REST.Dto;
using Asp_REST.Entity;
using Asp_REST.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace Asp_REST.Services
{
    public class AdvertisementService (AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        public List<AdvertisementShowDto> GetAllAdvertisements()
        {
            var advertisements = context.Advertisements.OrderBy(advertisement => advertisement.Title).ToList();
            return advertisements.Select(advertisement => mapper.Map<AdvertisementShowDto>(advertisement)).ToList();
        }
        public List<AdvertisementShowDto> GetAdvertisementsByTitle(string title)
        {
            var advertisements = context.Advertisements
                .Where(l => EF.Functions.Like(l.Title.ToLower(), $"%{title.ToLower()}%"))  
                .OrderBy(l => l.Title).ToList();

            return advertisements.Select(advertisement => mapper.Map<AdvertisementShowDto>(advertisement)).ToList();
        }

        public List<AdvertisementShowDto> GetAdvertisementsByUserEmail(string email)
        {
            var advertisements = context.Advertisements.Where(l => l.User.Email == email)
                .OrderBy(l => l.Title).ToList();
            return advertisements.Select(advertisement => mapper.Map<AdvertisementShowDto>(advertisement)).ToList();
        }

        public List<AdvertisementShowDto> GetAdvertisementsByUserId(long userId)
        {
            var advertisements = context.Advertisements.Where(l => l.UserId == userId).
                OrderBy(l => l.Title).ToList();
            return advertisements.Select(advertisement => mapper.Map<AdvertisementShowDto>(advertisement)).ToList();
        }
        public AdvertisementShowDto CreateAdvertisement(AdvertisementCreationDto advertisementCreationDto)
        {
            var userEmail = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            var currentUser = context.Users.FirstOrDefault(u => u.Email == userEmail)
                ?? throw new UserException("Wystąpił problem z tokenem jwt");

            var date = DateTime.Now.ToString("dd-MM-yyyy HH:mm"); 

            var advertisement = new Advertisement
            {
                UserId = currentUser.Id,
                Title = advertisementCreationDto.Title,
                Description = advertisementCreationDto.Description,
                Price = advertisementCreationDto.Price,
                Duration = advertisementCreationDto.Duration,
                CreatedAt = date 
            };

            context.Advertisements.Add(advertisement);
            context.SaveChanges();

            return new AdvertisementShowDto(advertisement);
        }
        public AdvertisementShowDto EditAdvertisement(AdvertisementCreationDto advertisementCreationDto, long id)
        {
            var userEmail = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            var currentUser = context.Users.FirstOrDefault(u => u.Email == userEmail)
                ?? throw new UserException("Wystąpił problem z tokenem jwt");

            var advertisement = context.Advertisements.FirstOrDefault(l => l.Id == id)
                ?? throw new AdvertisementException("Brak advertisementu o id:" + id);

            if (advertisement.User.Email != userEmail && currentUser.Role != Role.Admin.ToString())
                throw new AdvertisementException("Tylko administrator może usuwać ogłoszenia innych użytkowników");

            advertisement.Title = advertisementCreationDto.Title;
            advertisement.Description = advertisementCreationDto.Description;
            advertisement.Price = advertisementCreationDto.Price;
            advertisement.Duration = advertisementCreationDto.Duration;

            context.Advertisements.Update(advertisement);
            context.SaveChanges();

            return mapper.Map<AdvertisementShowDto>(advertisement); 
        }

        public string DeleteAdvertisement(long id)
        {
            var userEmail = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            var currentUser = context.Users.FirstOrDefault(u => u.Email == userEmail)
                ?? throw new UserException("Wystąpił problem z tokenem jwt");

            var advertisement = context.Advertisements.FirstOrDefault(l => l.Id == id)
                ?? throw new AdvertisementException($"Brak advertisementu o id: {id}");

            if (advertisement.User.Email != userEmail && currentUser.Role != Role.Admin.ToString())
                throw new AdvertisementException("Tylko administrator może usuwać ogłoszenia innych użytkowników");

            context.Advertisements.Remove(advertisement);
            context.SaveChanges();

            return advertisement.Title; 
        }
    }
}
