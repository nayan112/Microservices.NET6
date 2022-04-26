using CommandService.Models;
using CommandService.SyncDataService.Grpc;

namespace CommandService.Data
{
    public static class PreparationDb
    {
        public static void PrePopulationDb(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
            }
        }

        private static void SeedData(ICommandRepository commandRepository, Task<IEnumerable<Platform>> platforms)
        {
            Console.WriteLine("Seeding new platform");
            foreach (var plat in platforms.Result)
            {
                if (!commandRepository.ExternalPlatformExists(plat.ExternalId))
                {
                    Console.WriteLine($"Seeding platform {plat.Name}");
                    commandRepository.CreatePlatform(plat);
                }
                commandRepository.SaveChanges();
            }
        }
    }
}