using System.Text;
using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IAssignmentHistoryMessageBuilderService
    {
        public StringBuilder Build();
        IAssignmentHistoryMessageBuilderService UseAddFilesMessage(AssignmentFile file);
        public IAssignmentHistoryMessageBuilderService UseRemoveFilesMessage(AssignmentFile file);
        public IAssignmentHistoryMessageBuilderService UseChangeStatusMessage(string from, string to);
        public IAssignmentHistoryMessageBuilderService UseAddResponsibleExecutorMessage(string added);
        public IAssignmentHistoryMessageBuilderService UseRemoveResponsibleExecutorMessage(string deleted);
        public IAssignmentHistoryMessageBuilderService UseChangeResponsibleExecutorMessage(string from, string to);
        public IAssignmentHistoryMessageBuilderService UseAddResponsibleLeaderMessage(string added);
        public IAssignmentHistoryMessageBuilderService UseRemoveResponsibleLeaderMessage(string deleted);
        public IAssignmentHistoryMessageBuilderService UseChangeResponsibleLeaderMessage(string from, string to);
        public IAssignmentHistoryMessageBuilderService UseAddResponsibleInspectorMessage(string added);
        public IAssignmentHistoryMessageBuilderService UseRemoveResponsibleInspectorMessage(string deleted);
        public IAssignmentHistoryMessageBuilderService UseChangeResponsibleInspectorMessage(string from, string to);
        public IAssignmentHistoryMessageBuilderService UseAddAssignmentAuthorMessage(string added);
        public IAssignmentHistoryMessageBuilderService UseRemoveAssignmentAuthorMessage(string deleted);
        public IAssignmentHistoryMessageBuilderService UseChangeAssignmentAuthorMessage(string from, string to);
    }
}