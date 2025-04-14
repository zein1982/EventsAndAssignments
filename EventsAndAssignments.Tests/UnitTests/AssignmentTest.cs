using EventsAndAssignments.API.Configs;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Data;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    public class AssignmentTest
    {
        #region Инициализация

        private const long _protocolId = 1;
        private const string _email = "rinat.salimianov@evraz.com"; //email текущего пользователя системы
        private readonly Guid _responsibleRinat = Guid.Parse("fc447276-cbef-ed11-b3d6-0050569a16c1");
        private readonly Guid _responsiblePavel = Guid.Parse("34cbf4a3-a9e8-ed11-b3d6-0050569a16c1");
        private readonly List<Assignment> _assignments = new();
        private readonly List<AssignmentStatus> _assignmentsStatuses = new();
        private readonly List<CommentResponseDto> _comments = new();
        private long _idCounter;
        private IAssignmentsService? _assignmentService;
        Mapper? _mapper;

        [SetUp]
        public void SetUp()
        {
            Mock<IAssignmentsGateway> assignmentRepositoryMock = new();
            Mock<IEmployeeService> employeeService = new();
            Mock<IAssignmentHistoryService> assignmentHistoryMock = new();
            Mock<ICommentService> commentServiceMock = new();
            Mock<IFileService> fileServiceMock = new();
            Mock<INotificationService> notificationServiceMock = new();
            Mock<ILogger<AssignmentsService>> logMock = new();
            _mapper = new Mapper(MappingConfig.GetConfig());

            FillAssignmentStatuses();
            SetupUserServiceMock(employeeService);
            SetupRepositoryMocks(assignmentRepositoryMock);
            SetupCommentServiceMock(commentServiceMock);
            SetupFileServiceMock(fileServiceMock);

            //Создание сервиса
            _assignmentService = new AssignmentsService(
                repository: assignmentRepositoryMock.Object,
                assignmentHistoryService: assignmentHistoryMock.Object,
                employeeService: employeeService.Object,
                commentService: commentServiceMock.Object,
                fileService: fileServiceMock.Object,
                mapper: _mapper,
                notificationService: notificationServiceMock.Object,
                logger: logMock.Object);
        }

        #endregion Инициализация

        #region Получение данных поручений

        [Test]
        [Order(0)]
        public async Task CreateAssignment_ShouldCreateAssignmentAndReturnShortNameTest()
        {
            //Act
            AssignmentResponseShort one = await _assignmentService!.CreateAssignmentAsync(_protocolId, _email);
            AssignmentResponseShort two = await _assignmentService.CreateAssignmentAsync(_protocolId, _email);
            AssignmentResponseShort three = await _assignmentService.CreateAssignmentAsync(_protocolId, _email);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(one.Name, Is.EqualTo("П-1"));
                Assert.That(two.Name, Is.EqualTo("П-2"));
                Assert.That(three.Name, Is.EqualTo("П-3"));
            });
        }

        [Test]
        public async Task GetAssignmentById_ShouldReturnValidAssignmentTest()
        {
            //Arrange
            const long id = 2;
            const int index = 1;

            //Act
            AssignmentResponse? res = await _assignmentService!.GetAssignmentById(id);

            //Assert
            Assert.That(res!.Id, Is.EqualTo(_assignments[index].Id));
        }

        [Test]
        [Order(1)]
        public async Task GetAssignmentByGroupIdAndVersion_ShouldReturnValidAssignmentTest()
        {
            //Arrange
            const long groupId = 1;
            const int version = 1;
            const int subversion = 0;
            const int index = 0;

            //Act
            AssignmentResponse? res = await _assignmentService!.GetAssignmentByGroupIdAndVersionAsync(groupId, version, subversion);

            //Assert
            Assert.That(res!.Id, Is.EqualTo(_assignments![index].Id));
        }

        #endregion Получение данных поручений

        #region Копирование поручений

        [Test]
        public async Task CopyAssignmentTest()
        {
            List<long> assignmentIds = new() { 1, 2 };
            ICollection<AssignmentResponseShort> assignments = await _assignmentService!.CopyAssignmentsAsync(assignmentIds, _protocolId, _email);
            Assert.That(assignmentIds.Count, Is.EqualTo(assignments!.Count));
        }

        #endregion Копирование поручений

        #region Тесты на продвижение поручения по статусам (основной поток, валидные данные)

        [Test]
        [Order(2)]
        [Repeat(3)]
        public async Task UpdateAssignmentAsync_ShouldReturnUpdatedAssignmentInStatusAssignTest()
        {
            //Arrange
            Assignment first = _assignments[0];
            AssignmentShortRequestDto dto = new()
            {
                Id = first.Id,
                Created = first.Created,
                Updated = DateTime.UtcNow,
                CreatedBy = first.CreatedBy,
                UpdatedBy = first.UpdatedBy,
                IsChecked = false,
                Description = first.Description,
                ResponsibleLeaderId = _responsibleRinat,
                CompanyId = first.OrganizationId,
                ProtocolId = first.ProtocolId,
                ExecutionDate = DateTime.Now + TimeSpan.FromDays(3),
                Comment = "test comment"
            };

            //Act
            AssignmentResponseShort updated = await _assignmentService!.UpdateAssignmentAsync(dto, _email, false);

            //Assert
            Assert.That(updated.Status, Is.EqualTo((int)Status.Assign));
        }

        [Test]
        [Order(3)]
        public async Task UpdateAssignmentAsync_ShouldReturnUpdatedAssignmentInStatusInWorkTest()
        {
            //Arrange
            Assignment first = _assignments[0];
            AssignmentRequestDto dto = new()
            {
                Id = first.Id,
                Created = first.Created,
                Updated = DateTime.UtcNow,
                CreatedBy = first.CreatedBy,
                UpdatedBy = first.UpdatedBy,
                Description = first.Description,
                AuthorId = _responsibleRinat,
                ResponsibleLeaders = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = _responsibleRinat,
                        ExecutionDate = DateTime.Now + TimeSpan.FromDays(3)
                    }
                },
                StatusId = first.StatusId,
                CompanyId = first.OrganizationId,
                ProtocolId = first.ProtocolId,
                Subversion = first.Subversion,
                Version = first.Version,
                NeedToReturnForRevision = false
            };

            //Act
            AssignmentResponse updated = await _assignmentService!.UpdateAssignmentAsync(dto, _email, dto.NeedToReturnForRevision.Value);

            //Assert
            Assert.That(updated.Status, Is.EqualTo((int)Status.InWork));
        }

        [Test]
        [Order(4)]
        public async Task UpdateAssignmentAsync_ShouldReturnUpdatedAssignmentInStatusCompletedTest()
        {
            //Arrange
            Assignment first = _assignments[0];
            AssignmentRequestDto dto = new()
            {
                Id = first.Id,
                Created = first.Created,
                Updated = DateTime.UtcNow,
                CreatedBy = first.CreatedBy,
                UpdatedBy = first.UpdatedBy,
                Description = first.Description,
                AuthorId = first.AuthorId,
                ResponsibleLeaders = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = first.ResponsibleLeaderId!.Value,
                        ExecutionDate = first.LeaderExecutionDate!.Value
                    }
                },
                StatusId = first.StatusId,
                CompanyId = first.OrganizationId,
                ProtocolId = first.ProtocolId,
                Subversion = first.Subversion,
                Version = first.Version,
                NeedToReturnForRevision = false
            };

            //Act
            AssignmentResponse updated = await _assignmentService!.UpdateAssignmentAsync(dto, _email, dto.NeedToReturnForRevision.Value);

            //Assert
            Assert.That(updated.Status, Is.EqualTo((int)Status.Completed));
        }

        [Test]
        [Order(5)]
        public async Task UpdateAssignmentAsync_ShouldReturnUpdatedAssignmentInStatusInWorkWhenReturnForRevisionTest()
        {
            //Arrange
            Assignment first = _assignments[0];
            AssignmentRequestDto dto = new()
            {
                Id = first.Id,
                Created = first.Created,
                Updated = DateTime.UtcNow,
                CreatedBy = first.CreatedBy,
                UpdatedBy = first.UpdatedBy,
                Description = first.Description,
                AuthorId = first.AuthorId,
                ResponsibleLeaders = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = first.ResponsibleLeaderId!.Value,
                        ExecutionDate = first.LeaderExecutionDate!.Value
                    }
                },
                StatusId = first.StatusId,
                CompanyId = first.OrganizationId,
                ProtocolId = first.ProtocolId,
                Subversion = first.Subversion,
                Version = first.Version,
                NeedToReturnForRevision = true
            };

            //Act
            AssignmentResponse updated = await _assignmentService!.UpdateAssignmentAsync(dto, _email, dto.NeedToReturnForRevision.Value);

            //Assert
            Assert.That(updated.Status, Is.EqualTo((int)Status.InWork));
        }

        [Test]
        [Order(6)]
        public async Task UpdateAssignmentAsync_ShouldReturnUpdatedAssignmentInStatusMonitoringTest()
        {
            //Arrange
            Assignment first = _assignments[0];
            AssignmentRequestDto dto = new()
            {
                Id = first.Id,
                Created = first.Created,
                Updated = DateTime.UtcNow,
                CreatedBy = first.CreatedBy,
                UpdatedBy = first.UpdatedBy,
                Description = first.Description,
                AuthorId = first.AuthorId,
                ResponsibleLeaders = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = first.ResponsibleLeaderId!.Value,
                        ExecutionDate = first.LeaderExecutionDate!.Value
                    }
                },
                ResponsibleExecutors = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = first.ResponsibleLeaderId!.Value,
                        ExecutionDate =  first.LeaderExecutionDate!.Value
                    }
                },
                ResponsibleInspectors = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = _responsiblePavel,
                        ExecutionDate = DateTime.Now + TimeSpan.FromDays(2)
                    }
                },
                StatusId = first.StatusId,
                CompanyId = first.OrganizationId,
                ProtocolId = first.ProtocolId,
                Subversion = first.Subversion,
                Version = first.Version,
                NeedToReturnForRevision = false
            };

            //Act
            AssignmentResponse updated = await _assignmentService!.UpdateAssignmentAsync(dto, _email, dto.NeedToReturnForRevision.Value);

            //Assert
            Assert.That(updated.Status, Is.EqualTo((int)Status.Monitoring));
        }

        [Test]
        [Order(7)]
        public async Task UpdateAssignmentAsync_ShouldReturnUpdatedAssignmentInStatusVerifiedTest()
        {
            //Arrange
            Assignment first = _assignments[0];
            AssignmentRequestDto dto = new()
            {
                Id = first.Id,
                Created = first.Created,
                Updated = DateTime.UtcNow,
                CreatedBy = first.CreatedBy,
                UpdatedBy = first.UpdatedBy,
                Description = first.Description,
                AuthorId = first.AuthorId,
                ResponsibleLeaders = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = first.ResponsibleLeaderId!.Value,
                        ExecutionDate = first.LeaderExecutionDate!.Value
                    }
                },
                ResponsibleInspectors = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = _responsiblePavel,
                        ExecutionDate = DateTime.Now + TimeSpan.FromDays(2)
                    }
                },
                StatusId = first.StatusId,
                CompanyId = first.OrganizationId,
                ProtocolId = first.ProtocolId,
                Subversion = first.Subversion,
                Version = first.Version,
                NeedToReturnForRevision = false
            };

            //Act
            AssignmentResponse updated = await _assignmentService!.UpdateAssignmentAsync(dto, _email, dto.NeedToReturnForRevision.Value);

            //Assert
            Assert.That(updated.Status, Is.EqualTo((int)Status.Verified));
        }

        [Test]
        [Order(8)]
        public async Task UpdateAssignmentAsync_ShouldReturnUpdatedAssignmentInStatusDoneTest()
        {
            //Arrange
            Assignment first = _assignments[0];
            AssignmentRequestDto dto = new()
            {
                Id = first.Id,
                Created = first.Created,
                Updated = DateTime.UtcNow,
                CreatedBy = first.CreatedBy,
                UpdatedBy = first.UpdatedBy,
                Description = first.Description,
                AuthorId = first.AuthorId,
                ResponsibleLeaders = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = first.ResponsibleLeaderId!.Value,
                        ExecutionDate = first.LeaderExecutionDate!.Value
                    }
                },
                ResponsibleInspectors = new List<ResponsibleRequest?>
                {
                    new()
                    {
                        EmployeePositionId = _responsiblePavel,
                        ExecutionDate = DateTime.Now + TimeSpan.FromDays(2)
                    }
                },
                StatusId = first.StatusId,
                CompanyId = first.OrganizationId,
                ProtocolId = first.ProtocolId,
                Subversion = first.Subversion,
                Version = first.Version,
                NeedToReturnForRevision = false
            };

            //Act
            _ = await _assignmentService!.UpdateAssignmentAsync(dto, _email, dto.NeedToReturnForRevision.Value);
            AssignmentResponse updated = await _assignmentService!.UpdateAssignmentAsync(dto, _email, dto.NeedToReturnForRevision.Value);

            //Assert
            Assert.That(updated.Status, Is.EqualTo((int)Status.Done));
        }

        #endregion Тесты на продвижение поручения по статусам (основной поток, валидные данные)

        #region Приватные мметоды

        private void SetupRepositoryMocks(Mock<IAssignmentsGateway> assignmentRepositoryMock)
        {
            //Мок на репозиторий поручений (создание)
            assignmentRepositoryMock.Setup(x => x
                    .CreateAssignmentAsync(It.IsAny<Assignment>()))
                .ReturnsAsync((Assignment s) =>
                {
                    s.Id = ++_idCounter;
                    s.Protocol = new Protocol { Id = s.ProtocolId, Name = $"Протокол № {s.ProtocolId}" };
                    _assignments!.Add(s);
                    return _assignments.Last();
                });

            //Мок на репозиторий поручений (получение по id)
            assignmentRepositoryMock.Setup(x => x
                    .GetAssignmentByIdAsync(It.IsAny<long>()))
                .ReturnsAsync((long id) => _assignments!.Find(e => e.Id == id));

            //Мок на репозиторий поручений (получение по id)
            assignmentRepositoryMock.Setup(x => x
                    .GetAssignmentByIdWithFilesAsync(It.IsAny<long>()))
                .ReturnsAsync((long id) => _assignments!.Find(e => e.Id == id));

            //Мок на репозиторий поручений (получение поручение по принадлежности к группе поручений, версии и подверсии)
            assignmentRepositoryMock.Setup(x => x
                    .GetAssignmentByGroupIdAndVersionAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((long groupId, int version, int subversion) =>
                {
                    Assignment finded = _assignments!.First(
                        x => x.GroupId == groupId
                            && x.Version == version
                            && x.Subversion == subversion);

                    return finded;
                });

            //Мок на репозиторий поручений (Получение количества поручений в протоколе)
            assignmentRepositoryMock.Setup(x => x
                    .GetAssignmentsCountInProtocol(It.IsAny<long>()))
                .Returns((long protocolId) => _assignments.Count(e => e.ProtocolId == protocolId));

            //Мок на репозиторий поручений (Получение статуса поручения по id)
            assignmentRepositoryMock.Setup(x => x
                    .GetAssignmentStatusByStatusCode(It.IsAny<int>()))
                .ReturnsAsync((int statusCode) => _assignmentsStatuses.Find(e => e.StatusCode == statusCode));

            //Мок на репозиторий поручений (Обновление поручений)
            assignmentRepositoryMock.Setup(x => x
                    .UpdateAssignmentAsync(It.IsAny<Assignment>()))
                .ReturnsAsync((Assignment assignment) =>
                {
                    int index = _assignments!.IndexOf(assignment);
                    _assignments[index] = assignment;
                    return assignment;
                });
        }

        private void SetupUserServiceMock(Mock<IEmployeeService> employeeService) =>
            employeeService.Setup(x => x
                    .GetEmployeeByEmail(It.IsAny<string>()))
                .Returns((string email) => new Models.DTO.Common.Employee
                {
                    Id = Guid.Parse("fc447276-cbef-ed11-b3d6-0050569a16c1"),
                    Email = email,
                    FullName = "Салимьянов Ринат Ильгизович",
                    Position = "ГЛАВНЫЙ СПЕЦИАЛИСТ",
                    Department = "УПРАВЛЕНИЕ РАЗРАБОТКИ ИС",
                    Organization = "ООО \"ЕвразТехника ИС\"",
                    PersonnelNumber = "07083875",
                    RoleId = 1
                });

        private void SetupCommentServiceMock(Mock<ICommentService> commentService)
        {
            commentService.Setup(x => x
                    .CreateAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), 1))
                .ReturnsAsync((string content, long assignmentId, string email) =>
                {
                    CommentResponseDto response = new()
                    {
                        Content = content,
                        AuthorFullName = email,
                        Created = DateTime.UtcNow
                    };
                    _comments.Add(response);

                    return response;
                });

            commentService.Setup(x => x
                    .GetLastAsync(It.IsAny<long>()))
                .ReturnsAsync((long assignmentId) => null);
        }

        private void SetupFileServiceMock(Mock<IFileService> commentService) =>
            commentService.Setup(x => x
                    .UploadFileToDbAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync((string content, byte[] fileBody, long assignmentId, string email) => new UploadFileToDbResponseDto());

        private void FillAssignmentStatuses()
        {
            _assignmentsStatuses.Add(new() { Id = 1, Name = "Новое", StatusCode = 1, IsInShortLine = true });
            _assignmentsStatuses.Add(new() { Id = 2, Name = "Назначено", StatusCode = 2, IsInShortLine = true });
            _assignmentsStatuses.Add(new() { Id = 3, Name = "В работе", StatusCode = 3, IsInShortLine = true });
            _assignmentsStatuses.Add(new() { Id = 4, Name = "Контроль", StatusCode = 4, IsInShortLine = false });
            _assignmentsStatuses.Add(new() { Id = 5, Name = "Проверка", StatusCode = 5, IsInShortLine = false });
            _assignmentsStatuses.Add(new() { Id = 6, Name = "Исполнено", StatusCode = 6, IsInShortLine = true });
            _assignmentsStatuses.Add(new() { Id = 7, Name = "Готово", StatusCode = 7, IsInShortLine = true });
        }

        #endregion Приватные мметоды

    }
}