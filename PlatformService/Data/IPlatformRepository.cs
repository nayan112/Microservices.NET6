using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepository
    {
        IEnumerable<Platform> GetPlatforms();
        Platform GetPlatform(int id);
        void CreatePlatform(Platform platform);
        //void UpdatePlatform(Platform platform);
        //void DeletePlatform(int id);
        bool SaveChanges();
    }
}