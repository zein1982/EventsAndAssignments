using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Helpers
{
    public static class PermissionHelper
    {
        public static void PermissionsInAssignment(Models.DTO.Common.Employee emp, Assignment updated, Assignment unupdated)
        {
            if (unupdated.ResponsibleExecutorId != updated.ResponsibleExecutorId//проверяем изменен ли ответственный руководитель
                && (emp.RoleId != Role.Admin.Id || emp.Id != unupdated.ResponsibleLeaderId)
                && unupdated.StatusId != 3)
            {
                throw new InvalidOperationException("Вы не можете изменить ответственного исполнителя");
            }

            if (unupdated.ResponsibleLeader != updated.ResponsibleLeader//проверяем изменен ли ответственный руководитель
                && (emp.RoleId != Role.Admin.Id || emp.Id != unupdated.ResponsibleLeaderId)
                && unupdated.StatusId > 3)
            {
                throw new InvalidOperationException("Вы не можете изменить ответственного руководитедя");
            }
        }
    }
}