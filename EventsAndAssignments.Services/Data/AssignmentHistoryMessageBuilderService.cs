using System.Text;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Interfaces;

namespace EventsAndAssignments.Services.Data
{
    public class AssignmentHistoryMessageBuilderService : IAssignmentHistoryMessageBuilderService
    {
        private StringBuilder? _assignmentHistoryMessage;

        public AssignmentHistoryMessageBuilderService()
        {
            _assignmentHistoryMessage = new StringBuilder();
        }

        public StringBuilder Build()
        {
            StringBuilder assignmentHistoryMessage = _assignmentHistoryMessage!;

            _assignmentHistoryMessage = new StringBuilder();

            return assignmentHistoryMessage;
        }

        public IAssignmentHistoryMessageBuilderService UseAddFilesMessage(AssignmentFile file)
        {
            _assignmentHistoryMessage!.Append("Прикрепил(а) ")
                .Append(file.OriginName)
                .Append(' ')
                .Append(file.SafetyName);

            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseRemoveFilesMessage(AssignmentFile file)
        {
            _assignmentHistoryMessage!.Append("Удалил(а) ")
                .Append(file.OriginName)
                .Append(' ')
                .Append(file.SafetyName);

            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseAddResponsibleExecutorMessage(string added)
        {
            _assignmentHistoryMessage!.Append("Назначил(а) ответственного исполнителя (")
                .Append(added)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseRemoveResponsibleExecutorMessage(string deleted)
        {
            _assignmentHistoryMessage!.Append("Удалил(а) ответственного исполнителя (")
                .Append(deleted)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseChangeResponsibleExecutorMessage(string from, string to)
        {
            _assignmentHistoryMessage!.Append("Изменил(а) ответственного исполнителя c \"")
                .Append(from)
                .Append("\" на \"")
                .Append(to)
                .Append('\"');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseChangeStatusMessage(string from, string to)
        {
            _assignmentHistoryMessage!.Append("Изменил(а) статус поручения c \"")
                .Append(from)
                .Append("\" на \"")
                .Append(to)
                .Append('\"');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseAddResponsibleLeaderMessage(string added)
        {
            _assignmentHistoryMessage!.Append("Назначил(а) ответственного руководителя (")
                .Append(added)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseRemoveResponsibleLeaderMessage(string deleted)
        {
            _assignmentHistoryMessage!.Append("Удалил(а) ответственного руководителя (")
                .Append(deleted)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseChangeResponsibleLeaderMessage(string from, string to)
        {
            _assignmentHistoryMessage!.Append("Изменил(а) ответственного руководителя c \"")
                .Append(from)
                .Append("\" на \"")
                .Append(to)
                .Append('\"');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseAddResponsibleInspectorMessage(string added)
        {
            _assignmentHistoryMessage!.Append("Назначил(а) контролера (")
                .Append(added)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseRemoveResponsibleInspectorMessage(string deleted)
        {
            _assignmentHistoryMessage!.Append("Удалил(а) ответственного контролера (")
                .Append(deleted)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseChangeResponsibleInspectorMessage(string from, string to)
        {
            _assignmentHistoryMessage!.Append("Изменил(а) ответственного контролера c \"")
                .Append(from)
                .Append("\" на \"")
                .Append(to)
                .Append('\"');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseAddAssignmentAuthorMessage(string added)
        {
            _assignmentHistoryMessage!.Append("Назначил(а) автора поручения (")
                .Append(added)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseRemoveAssignmentAuthorMessage(string deleted)
        {
            _assignmentHistoryMessage!.Append("Удалил(а) автора поручения (")
                .Append(deleted)
                .Append(')');
            return this;
        }

        public IAssignmentHistoryMessageBuilderService UseChangeAssignmentAuthorMessage(string from, string to)
        {
            _assignmentHistoryMessage!.Append("Изменил(а) автора поручения c \"")
                .Append(from)
                .Append("\" на \"")
                .Append(to)
                .Append('\"');
            return this;
        }
    }
}