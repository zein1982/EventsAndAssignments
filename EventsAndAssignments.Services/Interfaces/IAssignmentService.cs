using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IAssignmentsService
    {
        Task<AssignmentResponse?> GetAssignmentById(long id, string? currentUserMail = "");
        Task<AssignmentResponse?> GetAssignmentByGroupIdAndVersionAsync(long groupId, int version, int subversion);
        Task<(List<AssignmentResponseShort> items, int count)> GetFilteredAssignments(RequestParams filter, string userMail);
        Task<List<long>> GetFilteredAssignmentsIds(RequestParams filter, string userMail);
        Task<ICollection<AssignmentVersionResponse>> GetAllAssignmentVersions([Required] long assignmentId);
        Task<int> GetAssignmentCountAsync(Func<Assignment, bool>? predicate = null);
        Task<AssignmentStatusResponse> GetAssignmentStatusByStatusCodeAsync(int statusCode);
        Task<ICollection<AssignmentStatusResponse>> GetAllAssignmentStatusesAsync(bool hasResponsibleInspector);
        Task<AssignmentResponseShort> CreateAssignmentAsync(long protocolId, string currentEmployeeEmail);
        Task<AssignmentResponseShort> UpdateAssignmentAsync(AssignmentShortRequestDto changed, string currentUserEmail, bool? needToReturnForRevision);
        Task<AssignmentResponse> UpdateAssignmentAsync(AssignmentRequestDto changed, string currentUserEmail, bool? needToReturnForRevision);
        Task RemoveAssignmentsAsync(IReadOnlyCollection<long> ids);
        Task<ICollection<AssignmentResponseShort>> CopyAssignmentsAsync(ICollection<long> assignmentIds, long protocolId, string currentEmployeeEmail);
        Task<string> RestoreNotificationsOnAssignments(ICollection<long> assignmentsIds, string currentUserEmail);
    }
}