using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.Interfaces;

namespace EventsAndAssignments.Services.Data
{
    public class DataSeedService : IDataSeedService
    {
        readonly IDataSeedGateway _dataSeedGateway;

        public DataSeedService(IDataSeedGateway dataSeedGateway)
        {
            _dataSeedGateway = dataSeedGateway;
        }

        public void Seed() => _dataSeedGateway.Seed();
    }
}