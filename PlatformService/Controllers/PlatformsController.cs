using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController(IPlatformRepo platformRepo, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms...");

        var platformItems = platformRepo.GetAllPlatforms();

        return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platformItem = platformRepo.GetPlatformById(id);
        if (platformItem is not null)
        {
            return Ok(mapper.Map<PlatformReadDto>(platformItem));
        }
        return NotFound();
    }

    [HttpPost]
    public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        var platform = mapper.Map<Platform>(platformCreateDto);
        platformRepo.CreatePlatform(platform);
        platformRepo.SaveChanges();

        var platformReadDto = mapper.Map<PlatformReadDto>(platform);

        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
    }
}
