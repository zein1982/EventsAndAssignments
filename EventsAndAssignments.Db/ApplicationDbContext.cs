using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EventsAndAssignments.Db
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connStr;
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentStatus> AssignmentStatuses { get; set; }
        public DbSet<ProtocolFolder> ProtocolFolders { get; set; }
        public DbSet<Protocol> Protocols { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AssignmentFile> Files { get; set; }
        public DbSet<AssignmentHistory> AssignmentHistories { get; set; }
        public DbSet<EmployeeDirectory> EmployeeDirectory { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PeriodicNotification> PeriodicNotifications { get; set; }
        public DbSet<NotificationSetting> NotificationSettings { get; set; }
        public DbSet<InstructuonFile> InstructuonFiles { get; set; }

        public ApplicationDbContext(string connectionString)
        {
            _connStr = connectionString;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connStr);
            }

            //optionsBuilder.LogTo(Console.WriteLine);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeDirectory>(entity =>
            {
                //entity
                //    .HasKey(e => e.PositionId);
                //entity
                //    .HasOne(e => e.EmployeeNavigation)
                //    .WithOne(e => e.EmployeeDirectoryNavigation)
                //    .HasForeignKey<Employee>(e => e.PositionId);
                //entity.Property(e => e.FullUserName)
                //    .HasComputedColumnSql(_fullNameQuery);
                //entity
                //    .HasIndex(e => e.FullUserName);
                //entity.ToView("View_EmployeeDirecotry");
                entity
                    .HasNoKey()
                    .ToView("ViewEmployeeDirectory");
            });

            // Трудозанятые
            modelBuilder.Entity<Employee>(entity =>
            {
                entity
                    .HasKey(e => e.PositionId);
                //entity
                //.HasOne(x => x.UserRole)
                //.WithMany(x => x.Employees)
                //.HasForeignKey(x => x.UserRoleId);
            });

            //Папки протоколов
            modelBuilder.Entity<ProtocolFolder>(entity =>
            {
                entity
                    .HasOne(e => e.CreatedByNavigation)
                    .WithMany(e => e.ProtocolFoldersCreatedByNavigation)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(e => e.UpdatedByNavigation)
                    .WithMany(e => e.ProtocolFoldersUpdatedByNavigation)
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasMany(e => e.AllowedEmployeesNavigation)
                    .WithMany(e => e.ProtocolFoldersAllowedEmployeesNavigation);
            });

            //Протоколы
            modelBuilder.Entity<Protocol>(entity =>
            {
                entity
                    .HasOne(e => e.Folder)
                    .WithMany(e => e.Protocols)
                    .HasForeignKey(e => e.FolderId);

                entity
                    .HasOne(e => e.CreatedByNavigation)
                    .WithMany(e => e.ProtocolsCreatedByNavigation)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(e => e.UpdatedByNavigation)
                    .WithMany(e => e.ProtocolsUpdatedByNavigation)
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //Поручения
            modelBuilder.Entity<Assignment>(entity =>
            {
                //Связи
                entity
                    .HasOne(e => e.Protocol)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.ProtocolId);

                entity
                    .HasOne(e => e.Status)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.StatusId);

                entity
                    .HasOne(e => e.Organization)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.OrganizationId);

                entity
                    .HasOne(e => e.Author)
                    .WithMany(e => e.AssignmentsAuthorNavigation)
                    .HasForeignKey(e => e.AuthorId);

                entity
                    .HasOne(e => e.ResponsibleLeader)
                    .WithMany(e => e.AssignmentsResponsibleLeaderNavigation)
                    .HasForeignKey(e => e.ResponsibleLeaderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(e => e.ResponsibleExecutor)
                    .WithMany(e => e.AssignmentsResponsibleExecutorNavigation)
                    .HasForeignKey(e => e.ResponsibleExecutorId);

                entity
                    .HasOne(e => e.ResponsibleInspector)
                    .WithMany(e => e.AssignmentsResponsibleInspectorNavigation)
                    .HasForeignKey(e => e.ResponsibleInspectorId);
                entity
                    .HasOne(e => e.CreatedByNavigation)
                    .WithMany(e => e.AssignmentsCreatedByNavigation)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(e => e.UpdatedByNavigation)
                    .WithMany(e => e.AssignmentsUpdatedByNavigation)
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //Комментарии
            modelBuilder.Entity<Comment>(entity =>
            {
                entity
                    .HasOne(e => e.Assignment)
                    .WithMany(e => e.Comments)
                    .HasForeignKey(e => e.AssignmentId);
                entity
                    .HasOne(e => e.CreatedByNavigation)
                    .WithMany(e => e.CommentsCreatedByNavigation)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(e => e.UpdatedByNavigation)
                    .WithMany(e => e.CommentsUpdatedByNavigation)
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //Файлы
            modelBuilder.Entity<AssignmentFile>(entity =>
            {
                entity
                    .HasOne(e => e.Assignment)
                    .WithMany(e => e.Files)
                    .HasForeignKey(e => e.AssignmentId);
                entity
                    .HasOne(e => e.CreatedByNavigation)
                    .WithMany(e => e.AssignmentFilesCreatedByNavigation)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(e => e.UpdatedByNavigation)
                    .WithMany(e => e.AssignmentFilesUpdatedByNavigation)
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //История поручений
            modelBuilder.Entity<AssignmentHistory>(entity =>
            {
                //Связи
                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.History)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AssignmentHistoryCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy);

                entity.HasOne(d => d.AddedResponsibleExecutorNavigation)
                    .WithMany(p => p.AssignmentHistoryAddedResponsibleExecutorNavigation)
                    .HasForeignKey(d => d.AddedResponsibleExecutor);

                entity.HasOne(d => d.RemovedResponsibleExecutorNavigation)
                    .WithMany(p => p.AssignmentHistoryRemovedResponsibleExecutorNavigation)
                    .HasForeignKey(d => d.RemovedResponsibleExecutor);

                entity.HasOne(d => d.FromStatusNavigation)
                    .WithMany(p => p.AssignmentHistoryFromStatus)
                    .HasForeignKey(d => d.FromStatus);

                entity.HasOne(d => d.ToStatusNavigation)
                    .WithMany(p => p.AssignmentHistoryToStatus)
                    .HasForeignKey(d => d.ToStatus);

                entity.HasOne(d => d.AddedFileNavigation)
                    .WithMany(p => p.AssignmentHistoryAddedFileNavigations)
                    .HasForeignKey(d => d.AddedFile);

                entity.HasOne(d => d.RemovedFileNavigation)
                    .WithMany(p => p.AssignmentHistoryDeletedFileNavigations)
                    .HasForeignKey(d => d.RemovedFile);
            });

            //Настройки уведомлений
            modelBuilder.Entity<NotificationSetting>(entity =>
            {
                entity.HasOne(e => e.UserNavigation)
                    .WithMany(x => x.NotificationSettingUserNavigation)
                    .HasForeignKey(e => e.UserPositionId);
            });

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.PeriodicNotification)
                .WithOne()
                .HasForeignKey<Notification>(n => n.PeriodicNotificationId)
                .OnDelete(DeleteBehavior.SetNull);

            ////Роли и разрешения
            //modelBuilder.Entity<Permission>(entity =>
            //{
            //    entity
            //        .HasOne(e => e.EmployeeRole)
            //        .WithMany(e => e.Permissions)
            //        .HasForeignKey(e => e.USerRoleId);
            //});
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.ToTable("Permissions");

                IEnumerable<Permission> permissions = Enum.GetValues<Services.Enums.Permission>()
                    .Select(x => new Permission { Id = (long)x, Name = x.ToString() });

                entity.HasData(permissions);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.ToTable("Roles");

                entity.HasMany(x => x.Permissions)
                    .WithMany(x => x.Roles)
                    .UsingEntity<RolePermission>();

                entity.HasMany(x => x.Employees)
                    .WithOne(x => x.UserRole);

                entity.HasData(Role.GetAll());
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(x => new { x.RoleId, x.PermissionId });

                List<RolePermission> data = new()
                {
                    Create(Role.Admin, Services.Enums.Permission.CreateFolder),
                    Create(Role.Admin, Services.Enums.Permission.ReadFolder),
                    Create(Role.Admin, Services.Enums.Permission.UpdateFolder),
                    Create(Role.Admin, Services.Enums.Permission.RemoveFolder),

                    Create(Role.SystemAdmin, Services.Enums.Permission.CreateFolder),
                    Create(Role.SystemAdmin, Services.Enums.Permission.ReadFolder),
                    Create(Role.SystemAdmin, Services.Enums.Permission.UpdateFolder),
                    Create(Role.SystemAdmin, Services.Enums.Permission.RemoveFolder),

                    Create(Role.Admin, Services.Enums.Permission.CreateProtocol),
                    Create(Role.Admin, Services.Enums.Permission.ReadProtocol),
                    Create(Role.Admin, Services.Enums.Permission.UpdateProtocol),
                    Create(Role.Admin, Services.Enums.Permission.RemoveProtocol),

                    Create(Role.SystemAdmin, Services.Enums.Permission.CreateProtocol),
                    Create(Role.SystemAdmin, Services.Enums.Permission.ReadProtocol),
                    Create(Role.SystemAdmin, Services.Enums.Permission.UpdateProtocol),
                    Create(Role.SystemAdmin, Services.Enums.Permission.RemoveProtocol),

                    Create(Role.Admin, Services.Enums.Permission.RemoveAssignment),
                    Create(Role.SystemAdmin, Services.Enums.Permission.RemoveAssignment),
                    Create(Role.Admin, Services.Enums.Permission.CreateAssignment),
                    Create(Role.SystemAdmin, Services.Enums.Permission.CreateAssignment)
                };

                entity.HasData(data);
            });
        }

        private RolePermission Create(Role role, Services.Enums.Permission permission) =>
            new() { RoleId = role.Id, PermissionId = (long)permission };

        private void OnBeforeSaving()
        {
            //Помечаем, когда была создана/модифицирована сущность
            IEnumerable<EntityEntry> modifiedEntries = ChangeTracker
                .Entries()
                .Where(e => e.State is not EntityState.Unchanged);

            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is not BaseEntity entity)
                {
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entity.Created == DateTime.MinValue || entity.Created == DateTime.MaxValue)
                        {
                            entity.Created = DateTime.UtcNow;
                        }

                        entity.Updated = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entity.Updated = DateTime.UtcNow;
                        break;

                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    case EntityState.Deleted:
                    default:
                        continue;
                }
            }
        }
    }
}