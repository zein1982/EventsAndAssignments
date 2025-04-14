using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class CommentRequestDto
    {
        public long AssignmentId { get; set; }
        public long StatusCard { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}