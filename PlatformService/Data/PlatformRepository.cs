using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository:IPlatformRepository
    {
        private readonly AppDbContext _context;

        public PlatformRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Platform> GetPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetPlatform(int id)
        {
            return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public void CreatePlatform(Platform platform)
        {
            if(platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            _context.Platforms.Add(platform);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}