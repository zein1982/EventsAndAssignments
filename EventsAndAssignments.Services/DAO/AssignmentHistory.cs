namespace EventsAndAssignments.Services.DAO
{
    public class AssignmentHistory
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Идентификатор поручения по которому ведется запись истории
        /// </summary>
        public long? AssignmentId { get; set; }

        /// <summary>
        /// Тип изменения поручения
        /// </summary>
        public int ModificationType { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Идентификатор создателя
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Прикрепленный к поручению ответственный
        /// </summary>
        public Guid? AddedResponsibleExecutor { get; set; }

        /// <summary>
        /// Открепленный от выполнения поручения ответственный
        /// </summary>
        public Guid? RemovedResponsibleExecutor { get; set; }

        /// <summary>
        /// Предшедствующий статус поручения
        /// </summary>
        public long? FromStatus { get; set; }

        /// <summary>
        /// Новый статус поручения
        /// </summary>
        public long? ToStatus { get; set; }

        /// <summary>
        /// Добавленный файл
        /// </summary>
        public long? AddedFile { get; set; }

        /// <summary>
        /// Удаленный файл
        /// </summary>
        public long? RemovedFile { get; set; }

        //Навигационные свойства

        public virtual Assignment? Assignment { get; set; }
        public virtual Employee? CreatedByNavigation { get; set; }
        public virtual Employee? AddedResponsibleExecutorNavigation { get; set; }
        public virtual Employee? RemovedResponsibleExecutorNavigation { get; set; }
        public virtual AssignmentStatus? FromStatusNavigation { get; set; }
        public virtual AssignmentStatus? ToStatusNavigation { get; set; }
        public virtual AssignmentFile? AddedFileNavigation { get; set; }
        public virtual AssignmentFile? RemovedFileNavigation { get; set; }
    }
}