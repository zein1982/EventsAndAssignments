using EventsAndAssignments.Services.Data;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    internal class ProtocolTest
    {
        ILogger<ProtocolService> _logger;
        ILogger<EmployeeService> _empLogger;
        IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            Mock<ILogger<ProtocolService>> logMock = new();
            Mock<ILogger<EmployeeService>> emplogMock = new();
            _logger = logMock.Object;
            _mapper = TestHelper.GetMapper();
            _empLogger = emplogMock.Object;
        }

        //[Test]
        //public async Task ProtocolCreateUpdateDeleteTestAsync()
        //{
        //    ApplicationDbContext ctx = TestHelper.GetTestDbContext();
        //    EmployeeGateway empRepo = new(ctx);
        //    EmployeeService emp = new(_empLogger, empRepo, _mapper);
        //    ProtocolGateway protocolRepo = new(ctx);
        //    ProtocolFoldersGateway folderRepo = new(ctx);
        //    ProtocolService service = new(protocolRepo, folderRepo, _mapper, emp);

        //    long folderId = ctx.ProtocolFolders.First().Id;

        //    CreateProtocolRequestDTO protocol1 = new() {CreatedBy = Guid.Parse("34cbf4a3-a9e8-ed11-b3d6-0050569a16c1"), FolderId = folderId, };

        //    CreateProtocolRequestDTO protocol2 = new() {CreatedBy = Guid.Parse("34cbf4a3-a9e8-ed11-b3d6-0050569a16c1"), FolderId = folderId, };

        //    CreateProtocolRequestDTO protocol3 = new() {CreatedBy = Guid.Parse("34cbf4a3-a9e8-ed11-b3d6-0050569a16c1"), FolderId = folderId, };

        //    Assert.That(protocol3, Is.Not.Null);

        //    Models.DTO.Response.CreateProtocolResponseDTO cr1 = await service.CreateAsync(protocol1, _mockMail);
        //    Models.DTO.Response.CreateProtocolResponseDTO cr2 = await service.CreateAsync(protocol2, _mockMail);
        //    Models.DTO.Response.CreateProtocolResponseDTO cr3 = await service.CreateAsync(protocol3, _mockMail);

        //    await service.RemoveProtocolsByAdmin(new List<long> { cr1.Id, cr2.Id, cr3.Id });
        //}
    }
}