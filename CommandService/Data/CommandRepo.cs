using CommandService.Models;

namespace CommandService.Data;
public class CommandRepo(AppDbContext context) : ICommandRepo
{
    public void CreateCommand(int platformId, Command command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }
        command.PlatformId = platformId;
        context.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform == null)
        {
            throw new ArgumentNullException(nameof(platform));
        }
        context.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return context.Platforms.ToList();
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return context.Commands
            .Where(x => x.PlatformId == platformId && x.Id == commandId)
            .FirstOrDefault();
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return context.Commands
            .Where(x => x.PlatformId == platformId)
            .OrderBy(x => x.Platform.Name);
    }

    public bool PlatformExists(int platformId)
    {
        return context.Platforms.Any(x => x.Id == platformId);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        return context.Platforms.Any(x => x.ExternalId == externalPlatformId);
    }

    public bool SaveChanges()
    {
        return context.SaveChanges() >= 0;
    }
}