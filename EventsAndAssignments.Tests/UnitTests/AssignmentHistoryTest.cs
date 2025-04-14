using EventsAndAssignments.Db;
using EventsAndAssignments.Db.Repositories;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.Data;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    //internal class AssignmentHistoryTest
    //{
    //    const int _assignmentId = 1;

    //    private readonly ILogger<AssignmentHistoryService>? _historyLogger;
    //    private IAssignmentHistoryService _historyService;
    //    private IEmployeeService _employeeService;
    //    private IAssignmentHistoryMessageBuilderService? _historyMessageBuilderService;
    //    private IAssignmentHistoryGateway _gateway;
    //    private ICollection<AssignmentHistoryMessage> _messages;

    //    [SetUp]
    //    public void Setup()
    //    {
    //        //Db context
    //        ApplicationDbContext dbContext = TestHelper.GetTestDbContext();

    //        //Моки 
    //        Mock<ILogger<AssignmentHistoryService>> logMockAssignmentHistory = new ();
    //        Mock<ILogger<EmployeeService>> logMockEmployee = new ();

    //        //Репозиторий
    //        _gateway = new AssignmentHistoryGateway(dbContext);

    //        //Сервисы
    //        _historyMessageBuilderService = new AssignmentHistoryMessageBuilderService();
    //        _employeeService = new EmployeeService(logMockEmployee.Object, "test");
    //        _historyService = new AssignmentHistoryService(
    //            _gateway,
    //            _historyMessageBuilderService,
    //            _employeeService,
    //            logMockAssignmentHistory.Object);

    //        _messages = new List<AssignmentHistoryMessage>();
    //    }

    //    [Test]
    //    public async Task ReturnsValidMessageWhenAddAndRemoveFile()
    //    {
    //        //Arrange
    //        AssignmentFile file = new() { OriginName = "Файл1", SafetyName = "Файл1", Content = new byte[1000] };

    //        //Act
    //        AssignmentHistoryMessage add = await _historyService.CreateFromAssignmentFilesModificationAsync(file, FileAction.Add);
    //        AssignmentHistoryMessage remove = await _historyService.CreateFromAssignmentFilesModificationAsync(file, FileAction.Remove);
    //        _messages.Add(add);
    //        _messages.Add(remove);

    //        //Assert
    //        Assert.That(add.Description, Is.EqualTo("Прикрепил(а) Файл1(1Kb)"));
    //        Assert.That(remove.Description, Is.EqualTo("Удалил(а) Файл1(1Kb)"));
    //    }

    //    [Test]
    //    public async Task GetAllTestAsync()
    //    {
    //        //Act
    //        ICollection<AssignmentHistoryMessage> res = await _historyService.GetAllAsync(_assignmentId);

    //        //Assert
    //        Assert.That(res, Has.Count.EqualTo(expected: _messages.Count));
    //    }
    //}
}