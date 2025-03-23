using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("/api/c/platforms/{platformId}/[controller]")]
public class CommandsController(ICommandRepo repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

        if (!repository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var commands = repository.GetCommandsForPlatform(platformId);
        return Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

        if (!repository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = repository.GetCommand(platformId, commandId);
        if (command == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

        if (!repository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = mapper.Map<Command>(commandDto);
        repository.CreateCommand(platformId, command);
        repository.SaveChanges();

        var commandReadDto = mapper.Map<CommandReadDto>(command);

        return CreatedAtRoute(
            nameof(GetCommandForPlatform),
            new { platformId = platformId, commandId = commandReadDto.Id },
            commandReadDto
        );
    }
}
