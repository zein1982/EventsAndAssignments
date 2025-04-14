using EventsAndAssignments.API.Configs;
using EventsAndAssignments.Db;
using Mapster;
using MapsterMapper;

namespace EventsAndAssignments.Tests
{
    public static class TestHelper
    {
        public static ApplicationDbContext GetTestDbContext()
        {
            const string conStr = "Server=RUK-SQL-XD067.sib.evraz.com\\PRJ10252_DEV;Database=EventsAndAssignments;Trusted_Connection=True;Encrypt=False";

            ApplicationDbContext dbContext = new(conStr);

            return dbContext;
        }

        public static Mapper GetMapper()
        {
            TypeAdapterConfig mappingConfig = MappingConfig.GetConfig();
            return new Mapper(mappingConfig);
        }
    }
}