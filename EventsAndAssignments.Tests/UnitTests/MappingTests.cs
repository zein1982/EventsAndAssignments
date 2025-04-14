using EventsAndAssignments.API.Configs;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.DAO;
using Mapster;
using MapsterMapper;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    internal class MappingTests
    {
        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty.
        }

        [Test]
        public void MappingConfigTest()
        {
            TypeAdapterConfig config = MappingConfig.GetConfig();
            config.RequireExplicitMapping = true;
            config.RequireDestinationMemberSource = true;

            Assert.DoesNotThrow(() => config.Compile());
        }

        [Test]
        public void ProtocolFolderDTOTest()
        {
            MappingConfig.GetConfig();

            Services.DAO.ProtocolFolder source = new()
            {
                Id = 1,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                CreatedBy = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                Name = "name"
            };

            IMapper mapper = new Mapper();

            Models.DTO.Response.ProtocolFolder dto = mapper.Map<Models.DTO.Response.ProtocolFolder>(source);

            Assert.That(dto.Id, Is.EqualTo(1));

            List<Services.DAO.ProtocolFolder> folderList = new() { source, source };

            List<Models.DTO.Response.ProtocolFolder> dtoList = mapper.Map<List<Models.DTO.Response.ProtocolFolder>>(folderList);

            Assert.That(dtoList[0].Id, Is.EqualTo(1));
        }
    }
}