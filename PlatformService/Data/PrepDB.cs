using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(),isProd);
            }
        }
        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to seed data by migration..");
                try 
                { 
                    context.Database.Migrate(); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Error running migration: {ex.Message}");
                }
                context.Database.Migrate();
            }
            if(!context.Platforms.Any())
            {
                Console.WriteLine("Seeding data...");
                context.Platforms.AddRange(
                    new Platform { Name = "Dotnet",Publisher = "Microsoft",Cost = "Free" },
                    new Platform { Name = "Java",Publisher = "Oracle",Cost = "Free" },
                    new Platform { Name = "Python",Publisher = "Python",Cost = "Free" }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Platforms already exist in the database.");
            }
        }
    }
}