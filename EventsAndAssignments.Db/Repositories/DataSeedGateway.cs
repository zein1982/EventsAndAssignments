using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Db.Repositories
{
    public class DataSeedGateway : IDataSeedGateway
    {
        private const string _assignmentDescription =
            "Совершенствованию вентиляции шахт с учетом перспектив развития горных работ (5 лет), в т.ч. обеспечения Z и Кв, в которой обязательно предусмотреть:"
                + "–	модернизация существующих или строительство новых ВГП;"
                + "–	бурение скважин большого диаметра с поверхности для подачи свежей или выпуска исходящей струи;"
                + "–	увеличение сечения основных (капитальных) воздухоподающих и воздуховыдающих выработок;"
                + "–	изоляцию неиспользуемых выработок;"
                + "–	обеспечение 2й категории по степени устойчивости;"
                + "–	обеспечение 2й группы по состоянию проветривания;";

        private readonly ApplicationDbContext _context;

        public DataSeedGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            SeedAssignmentStatuses();
            SeedProtocolFolders();
            SeedProtocols();
            SeedAssignments();
        }

        private void SeedAssignmentStatuses()
        {
            if (!_context.AssignmentStatuses.Any())
            {
                List<AssignmentStatus> statuses = new()
                {
                    new() { Name = "Новое", StatusCode = 1, IsInShortLine = true },
                    new() { Name = "Назначено", StatusCode = 2, IsInShortLine = true },
                    new() { Name = "В работе", StatusCode = 3, IsInShortLine = true },
                    new() { Name = "Контроль", StatusCode = 4, IsInShortLine = false },
                    new() { Name = "Проверено", StatusCode = 5, IsInShortLine = false },
                    new() { Name = "Исполнено", StatusCode = 6, IsInShortLine = true },
                    new() { Name = "Готово", StatusCode = 7, IsInShortLine = true }
                };
                _context.AddRangeAsync(statuses);
                _context.SaveChanges();
            }
        }

        private void SeedProtocolFolders()
        {
            if (!_context.ProtocolFolders.Any())
            {
                List<ProtocolFolder> folders = new()
                {
                    new() { Name = "РУК-1", IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = "РУК-2", IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = "РУК-3", IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = "РУК-4", IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") }
                };
                _context.AddRangeAsync(folders);
                _context.SaveChanges();
            }
        }

        private void SeedProtocols()
        {
            if (!_context.Protocols.Any())
            {
                List<Protocol> protocols = new()
                {
                    new() { Name = $"Протокол №1 от {DateTime.UtcNow.Date} РУК-1", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = $"Протокол №2 от {DateTime.UtcNow.Date} РУК-1", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = $"Протокол №1 от {DateTime.UtcNow.Date} РУК-2", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = $"Протокол №2 от {DateTime.UtcNow.Date} РУК-2", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = $"Протокол №1 от {DateTime.UtcNow.Date} РУК-3", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = $"Протокол №2 от {DateTime.UtcNow.Date} РУК-3", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = $"Протокол №1 от {DateTime.UtcNow.Date} РУК-4", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                    new() { Name = $"Протокол №2 от {DateTime.UtcNow.Date} РУК-4", FolderId = GetRandomFolderId(), IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1") },
                };
                _context.AddRangeAsync(protocols);
                _context.SaveChanges();
            }
        }

        private void SeedAssignments()
        {
            if (!_context.Assignments.Any())
            {
                List<Assignment> assignments = new()
                {
                    new() { Name = "1", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                    new() { Name = "2", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                    new() { Name = "3", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                    new() { Name = "4", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                    new() { Name = "5", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                    new() { Name = "6", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                    new() { Name = "7", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                    new() { Name = "8", ProtocolId = GetRandomProtocolId(), StatusId = 1, Description = _assignmentDescription, IsArchived = false, CreatedBy = Guid.Parse("7dcbd559-4b25-ee11-b3d6-0050569a16c1"), Version = 1, Subversion = 0},
                };
                _context.AddRangeAsync(assignments);
                _context.SaveChanges();
            }
        }

        private long GetRandomFolderId() =>
            _context.ProtocolFolders
                .OrderBy(x => Guid.NewGuid()).First().Id;

        private long GetRandomProtocolId() =>
            _context.Protocols
                .OrderBy(x => Guid.NewGuid()).First().Id;
    }
}