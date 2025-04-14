using System.Text;
using EventsAndAssignments.Services.Data;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    public class AssignmentFileTest
    {
        readonly string _content = "Hello World from a Fake File";
        readonly string _fileName = "test"+DateTime.Now.Millisecond.ToString()+".pdf";
        ILogger<ProtocolFoldersService> _logger;
        IAssignmentHistoryService _historyService;
        IHostEnvironment _host;
        IFormFile _file;

        [SetUp]
        public void Setup()
        {
            Mock<ILogger<ProtocolFoldersService>> logMock = new();
            Mock<IAssignmentHistoryService> historyMock = new();
            Mock<IHostEnvironment> hostMock = new();
            Mock<IFormFile> fileMock = new();
            MemoryStream stream = new(Encoding.UTF8.GetBytes(_content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(_fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            _logger = logMock.Object;
            _host = hostMock.Object;
            _file = fileMock.Object;
            _historyService = historyMock.Object;
        }

        [Test]
        public void File_CRUD_Test()
        {
            //ApplicationDbContext ctx = TestHelper.GetTestDbContext();
            //FileGateway repo = new(_host, ctx);
            //AssignmentGateway assignmentGateway = new(ctx);
            //FileService service = new(repo, _historyService);
            //List<IFormFile> files = new() { _file };
            //long assignmentId = assignmentGateway.GetFilteredAssignments(null).First().Id;
            //ICollection<AssignmentFile> file1 = await service.UploadFileToDbAsync(files, assignmentId);

            ////тест создания
            //Assert.That(file1.ToList()[0].OriginName, Is.EqualTo(_fileName));

            ////AssignmentFile? removed = await service.RemoveFileAsync(file1.ToList()[0].Id);
            //////тест удаления
            ////Assert.That(removed, Is.Not.Null);
        }
    }
}