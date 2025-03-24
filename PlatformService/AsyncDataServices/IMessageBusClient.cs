using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices;

public interface IMessageBusClient
{
    ValueTask PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
}
