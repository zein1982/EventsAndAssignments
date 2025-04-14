namespace EventsAndAssignments.Models.DTO.Response
{
    public class CommentResponseDto
    {
        public long Id { get; set; }
        public string? Content { get; set; }
        public string? AuthorFullName { get; set; }
        public bool UserCanRemoveComment { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Removed { get; set; }

        /// <summary>
        /// Id пользователя который создал коммент
        /// </summary>
        public Guid CreatedBy { get; set; }
    }
}