namespace EventsAndAssignments.Services.DAO
{
    public class Assignment : BaseEntity
    {
        public DateTime ExecutionDate { get; set; }

        /// <summary>
        /// Дата исполнения ответственным руководителем
        /// </summary>
        public DateTime? LeaderExecutionDate { get; set; }

        /// <summary>
        /// Дата исполнения ответственным исполнителем
        /// </summary>
        public DateTime? ExecutorExecutionDate { get; set; }

        /// <summary>
        /// Дата исполнения ответственным контролером
        /// </summary>
        public DateTime? InspectorCheckDate { get; set; }

        /// <summary>
        /// Идентификатор группы поручений(для версионирования)
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// Наименование поручения (номер)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание поручения
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Подверсия
        /// </summary>
        public int Subversion { get; set; }

        /// <summary>
        /// Версия
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Признак архивной сущности (доступной только для чтения)
        /// </summary>
        public bool IsArchived { get; set; }

        //ключи
        /// <summary>
        /// Протокол, которому принадлежит поручение
        /// </summary>
        public long ProtocolId { get; set; }
        public Protocol? Protocol { get; set; }

        /// <summary>
        ///Статус поручения
        /// </summary>
        public long? StatusId { get; set; }
        public AssignmentStatus? Status { get; set; }

        /// <summary>
        /// Дата перехода поручения в статус "Готово"
        /// </summary>
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Организация в рамках которой создано поручение
        /// </summary>
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        /// <summary>
        /// Автор поручения
        /// </summary>
        public Guid? AuthorId { get; set; }
        public virtual Employee? Author { get; set; }

        /// <summary>
        /// Ответственный руководитель
        /// </summary>
        public Guid? ResponsibleLeaderId { get; set; }
        public virtual Employee? ResponsibleLeader { get; set; }

        /// <summary>
        /// Ответственный исполнитель
        /// </summary>
        public Guid? ResponsibleExecutorId { get; set; }
        public virtual Employee? ResponsibleExecutor { get; set; }

        /// <summary>
        /// Ответственный контролер
        /// </summary>
        public Guid? ResponsibleInspectorId { get; set; }
        public virtual Employee? ResponsibleInspector { get; set; }

        //навигационные свойства
        public Employee? CreatedByNavigation { get; set; }
        public Employee? UpdatedByNavigation { get; set; }
        public ICollection<AssignmentHistory>? History { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<AssignmentFile>? Files { get; set; }
        public ICollection<PeriodicNotification> PeriodicNotifications { get; set; } = new List<PeriodicNotification>();

        public bool IsAdmin(Guid currentEmployeePositionId) => currentEmployeePositionId == CreatedBy;
        public bool IsAuthor(Guid currentEmployeePositionId) => currentEmployeePositionId == AuthorId;
        public bool IsResponsibleLeader(Guid currentEmployeePositionId) => currentEmployeePositionId == ResponsibleLeaderId;
        public bool IsResponsibleExecutor(Guid currentEmployeePositionId) => currentEmployeePositionId == ResponsibleExecutorId;
        public bool IsResponsibleInspector(Guid currentEmployeePositionId) => currentEmployeePositionId == ResponsibleInspectorId;

        public Assignment GetCopy() =>
            new()
            {
                Id = Id,
                Created = Created,
                Updated = Updated,
                CreatedBy = CreatedBy,
                UpdatedBy = UpdatedBy,
                Removed = Removed,
                Name = Name,
                Description = Description,
                GroupId = GroupId,
                Subversion = Subversion,
                Version = Version,
                IsArchived = IsArchived,
                ProtocolId = ProtocolId,
                StatusId = StatusId,
                OrganizationId = OrganizationId,
                AuthorId = AuthorId,
                ResponsibleExecutorId = ResponsibleExecutorId,
                ResponsibleInspectorId = ResponsibleInspectorId,
                ResponsibleLeaderId = ResponsibleLeaderId,
                ExecutorExecutionDate = ExecutorExecutionDate,
                InspectorCheckDate = InspectorCheckDate,
                LeaderExecutionDate = LeaderExecutionDate,
                CompletionDate = CompletionDate,
            };
    }
}