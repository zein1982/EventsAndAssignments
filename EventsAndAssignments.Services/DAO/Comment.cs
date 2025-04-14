namespace EventsAndAssignments.Services.DAO
{
    public class Comment : BaseEntity
    {
        /// <summary>
        /// Содержание комментария
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Поле показывает в каком статусе поручения был создан комментарий
        /// </summary>
        public long? StatusCreated { get; set; }

        //Навигационные свойства
        public long? AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }
        public Employee? CreatedByNavigation { get; set; }
        public Employee? UpdatedByNavigation { get; set; }
    }
}