using Asp_REST.Data;
using Asp_REST.Dto;
using Asp_REST.Entity;
using Asp_REST.Exceptions;
using Asp_REST.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asp_REST.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AdvertisementController(AdvertisementService advertisementService, AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<AdvertisementShowDto>> GetAllAdvertisements()
        {
            var advertisements = advertisementService.GetAllAdvertisements();
            return Ok(advertisements);
        }

       [HttpGet("{id}")]
        public ActionResult<AdvertisementShowDto> GetAdvertisementById(long id)
        {
            var advertisement = context.Advertisements.FirstOrDefault(l => l.Id == id)
                ?? throw new AdvertisementException("Brak ogłoszenia o id:" + id);
            AdvertisementShowDto advertisementShowDto = new(advertisement);
            return Ok(advertisementShowDto);
        }
    

        [HttpGet("title")]
        public ActionResult<List<AdvertisementShowDto>> GetAdvertisementsByTitle([FromQuery] string title)
        {
            var advertisements = advertisementService.GetAdvertisementsByTitle(title);
            return Ok(advertisements);
        }

        [HttpGet("email")]
        public ActionResult<List<AdvertisementShowDto>> GetAdvertisementsByUserEmail([FromQuery] string email)
        {
            var advertisements = advertisementService.GetAdvertisementsByUserEmail(email);
            return Ok(advertisements);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<List<AdvertisementShowDto>> GetAdvertisementsByUserId(long userId)
        {
            var advertisements = advertisementService.GetAdvertisementsByUserId(userId);
            return Ok(advertisements);
        }

        [HttpPost]
        public ActionResult<AdvertisementShowDto> CreateAdvertisement([FromBody] AdvertisementCreationDto advertisementCreationDto)
        {
            var advertisement = advertisementService.CreateAdvertisement(advertisementCreationDto);
            return Ok(advertisement);
        }

        [HttpPatch("{id}")]
        public ActionResult<AdvertisementShowDto> EditAdvertisement([FromBody] AdvertisementCreationDto advertisementCreationDto, long id)
        {
            var advertisement = advertisementService.EditAdvertisement(advertisementCreationDto, id);
            return Ok(advertisement);
        }

        [HttpDelete("{id}")]
        public ActionResult<string> DeleteAdvertisement(long id)
        {
            var title = advertisementService.DeleteAdvertisement(id);
            return Ok($"Usunięto ogłoszenie o tytule: {title}");
        }
    }
}
