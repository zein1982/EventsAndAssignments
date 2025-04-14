using EventsAndAssignments.Db;
using EventsAndAssignments.Db.Repositories;
using EventsAndAssignments.Infrastructure;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Data;
using EventsAndAssignments.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    internal class NotyficationTests
    {
        ILogger<NotificationService> _logger;
        ILogger<FakeEmailSender> _loggerEmail;

        [SetUp]
        public void Setup()
        {
            Mock<ILogger<NotificationService>> logMock = new();
            _logger = logMock.Object;

            Mock<ILogger<FakeEmailSender>> logMock2 = new();
            _loggerEmail = logMock2.Object;
        }

        [Test]
        public void NotificationAddTest()
        {
            //ApplicationDbContext ctx = TestHelper.GetTestDbContext();
            //MapsterMapper.Mapper mapper=TestHelper.GetMapper();

            //NotificationGateway notifRepo = new(ctx);
            //EmployeeGateway emplRepo = new(ctx);
            //AssignmentsGateway assigRepo = new(ctx);
            ////FakeEmailSender fakeEmail= new(_loggerEmail);

            //Mock<ILogger<SmtpEmailSender>> logMock2 = new();
            //SmtpEmailSender fakeEmail= new(logMock2.Object);

            //NotificationService service = new(_logger, mapper, notifRepo, emplRepo, assigRepo, fakeEmail);

            //Assignment? assignment = await assigRepo.GetAssignmentByIdAsync(259);

            //AssignmentResponsibleManagerTemplate template= new(assignment);

            //NotificationSettingResponseDTO settings = await service.GetNotificationSettingsAsync(assignment.ResponsibleLeader.Email);
            ////Новое поручение
            //bool result = await service.AddNotificationAsync(template, settings);

            //List<Notification> result2= await service.GetNotifications(assignment.ResponsibleLeader.Email);

            //Assert.That(result2.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task NotificationSettingsTestAsync()
        {
            const string userEmail="Andrey.Safonov@evraz.com";
            ApplicationDbContext ctx = TestHelper.GetTestDbContext();
            MapsterMapper.Mapper mapper=TestHelper.GetMapper();

            NotificationGateway notifRepo = new(ctx);
            EmployeeGateway emplRepo = new(ctx);
            AssignmentsGateway assigRepo = new(ctx);
            FakeEmailSender fakeEmail= new(_loggerEmail);
            Mock<IOptions<NotificationsOptions>> options = new();
            options.Setup(e => e.Value.FrontUrl).Returns(() => "http://events-assignments-ruk-10252-dev.apps.ocpd.sib.evraz.com");
            NotificationService service = new(_logger, mapper, notifRepo, emplRepo, fakeEmail, options.Object);

            NotificationSettingResponseDTO set = await service.GetNotificationSettingsAsync(userEmail);

            Assert.That(set.IsNew, Is.EqualTo(true));

            NotificationSettingRequestDTO newSett = new()
            {
                IsNew=true,
                IsStatusChange=true,
                IsWeekly=true
            };

            bool result = await service.SetNotificationSettingsAsync(userEmail, newSett);

            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public async Task NotificationProcessTestAsync()
        {
            ApplicationDbContext ctx = TestHelper.GetTestDbContext();
            MapsterMapper.Mapper mapper=TestHelper.GetMapper();

            NotificationGateway notifRepo = new(ctx);
            EmployeeGateway emplRepo = new(ctx);
            AssignmentsGateway assigRepo = new(ctx);

            Mock<ILogger<SmtpEmailSender>> logMock = new();
            ILogger<SmtpEmailSender> loggerEmail = logMock.Object;

            SmtpEmailSender sender= new(loggerEmail);
            Mock<IOptions<NotificationsOptions>> options = new();
            options.Setup(e => e.Value.FrontUrl).Returns(() => "http://events-assignments-ruk-10252-dev.apps.ocpd.sib.evraz.com");

            NotificationService service = new(_logger, mapper, notifRepo, emplRepo, sender, options.Object);

            bool ret = await service.ProcessNotifications();

            Assert.That(ret, Is.EqualTo(true));
        }
    }
}