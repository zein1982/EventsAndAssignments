using EventsAndAssignments.Api.Configs;
using EventsAndAssignments.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EventAndAssignments.Migrator
{
    static class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            try
            {
                DbContextOptions<ApplicationDbContext> dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(ConnectionStringBuilder.GetConnectionStringForMigration(configuration)).Options;

                using (ApplicationDbContext db = new(dbOptions))
                {
                    db.Database.Migrate();
                    Console.WriteLine("Migrations are applied successfully.");
                }

                Environment.ExitCode = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Migration failed: " + e.Message);
                Console.WriteLine("Error source: " + e.Source);
                Console.WriteLine(e.StackTrace);

                Environment.ExitCode = 1;
            }
        }
    }
}