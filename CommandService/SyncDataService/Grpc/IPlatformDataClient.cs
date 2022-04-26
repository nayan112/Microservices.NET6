using CommandService.Models;

namespace CommandService.SyncDataService.Grpc
{
    public interface IPlatformDataClient
    {
        Task<IEnumerable<Platform>> ReturnAllPlatforms();
    }
}