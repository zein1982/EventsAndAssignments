using EventsAndAssignments.Models.DTO;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.DTO_GottaGetOutOfHere;
using Mapster;
using Microsoft.IdentityModel.Tokens;

namespace EventsAndAssignments.API.Configs
{
    public static class MappingConfig
    {
        /// <summary>
        /// Зададим глобальный конфиг. Проверка конфига в MappingTests
        /// </summary>
        public static TypeAdapterConfig GetConfig()
        {
            TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;

            //Совпадающие поля прописывать не нужно
            //Маппинг прописывается в таком формате
            //Это только для примера
            //https://github.com/MapsterMapper/Mapster/wiki/Configuration
            //config.NewConfig<Services.DAO.ProtocolFolder, Models.DTO.Response.ProtocolFolder>()
            //    .Map(dest => dest.Name, src => src.Name);

            //Common DTO

            config.NewConfig<AssignmentHistoryMessage, AssignmentHistoryResponseDto>();

            config.NewConfig<Employee, Models.DTO.Common.Employee>()
                .Map(dest => dest.Id, src => src.PositionId)
                .Map(dest => dest.FullName, src => src.GetFormatedName())
                .Map(dest => dest.Email, srs => srs.Email)
                .Map(dest => dest.Position, src => src.PositionName)
                .Map(dest => dest.Department, srs => srs.DepartmentName)
                .Map(dest => dest.Organization, srs => srs.OrganizationName)
                .Map(dest => dest.PersonnelNumber, srs => srs.TabelNumber);

            config.NewConfig<Models.DTO.Common.Employee, Employee>()
                .Map(dest => dest.PositionId, src => src.Id)
                .Ignore(dest => dest.TabelNumber!)
                .Ignore(dest => dest.Domain!)
                .Ignore(dest => dest.Email!)
                .Ignore(dest => dest.Login!)
                .Ignore(dest => dest.LastName!)
                .Ignore(dest => dest.FirstName!)
                .Ignore(dest => dest.MiddleName!)
                .Ignore(dest => dest.OrganizationCode!)
                .Ignore(dest => dest.OrganizationName!)
                .Ignore(dest => dest.EmployeeId)
                .Ignore(dest => dest.PositionCode!)
                .Ignore(dest => dest.PositionName!)
                .Ignore(dest => dest.DepartmentCode!)
                .Ignore(dest => dest.DepartmentName!)
                .Ignore(dest => dest.AssignmentsAuthorNavigation!)
                .Ignore(dest => dest.AssignmentsResponsibleExecutorNavigation!)
                .Ignore(dest => dest.AssignmentsResponsibleInspectorNavigation!)
                .Ignore(dest => dest.AssignmentsResponsibleLeaderNavigation!)
                .Ignore(dest => dest.AssignmentHistoryCreatedByNavigation!)
                .Ignore(dest => dest.AssignmentHistoryAddedResponsibleExecutorNavigation!)
                .Ignore(dest => dest.AssignmentHistoryRemovedResponsibleExecutorNavigation!)
                .Ignore(dest => dest.Photo!)
                .Ignore(dest => dest.PhotoS!)
                .Ignore(dest => dest.IsSfrelevant!)
                .Ignore(dest => dest.Occupation!)
                .Ignore(dest => dest.PersonLastModification)
                .Ignore(dest => dest.PositionLastModification)
                .Ignore(dest => dest.DepartmentLastModification)
                .Ignore(dest => dest.OrganizationLastModification)
                .Ignore(dest => dest.AnyLastModification)
                .Ignore(dest => dest.UserRole!)
                .Ignore(dest => dest.RoleId!)
                .Ignore(dest => dest.HireDate!)
                .Ignore(dest => dest.EndDate!)
                .Ignore(dest => dest.AnyLastModification!)
                .Ignore(dest => dest.ProtocolFoldersCreatedByNavigation!)
                .Ignore(dest => dest.ProtocolFoldersUpdatedByNavigation!)
                .Ignore(dest => dest.ProtocolsCreatedByNavigation!)
                .Ignore(dest => dest.ProtocolsUpdatedByNavigation!)
                .Ignore(dest => dest.AssignmentsCreatedByNavigation!)
                .Ignore(dest => dest.AssignmentsUpdatedByNavigation!)
                .Ignore(dest => dest.AssignmentFilesCreatedByNavigation!)
                .Ignore(dest => dest.AssignmentFilesUpdatedByNavigation!)
                .Ignore(dest => dest.CommentsCreatedByNavigation!)
                .Ignore(dest => dest.CommentsUpdatedByNavigation!)
                .Ignore(dest => dest.ProtocolFoldersAllowedEmployeesNavigation!)
                .Ignore(dest => dest.NotificationSettingUserNavigation!)
                .Ignore(dest => dest.IsActive);

            config.NewConfig<Employee, EmployeeWithAllPositionsDto>()
                .Map(dest => dest.Id, src => src.PositionId)
                .Map(dest => dest.FullName, src => src.GetFormatedName())
                .Map(dest => dest.Email, srs => srs.Email)
                .Map(dest => dest.Position, src => src.PositionName)
                .Map(dest => dest.Department, srs => srs.DepartmentName)
                .Map(dest => dest.Organization, srs => srs.OrganizationName)
                .Map(dest => dest.PersonnelNumber, srs => srs.TabelNumber)
                .Ignore(dest => dest.AllEmployeePositionsIds);

            config.NewConfig<EmployeeWithAllPositionsDto, Employee>()
                .Map(dest => dest.PositionId, src => src.Id)
                .Ignore(dest => dest.TabelNumber!)
                .Ignore(dest => dest.Domain!)
                .Ignore(dest => dest.Email!)
                .Ignore(dest => dest.Login!)
                .Ignore(dest => dest.LastName!)
                .Ignore(dest => dest.FirstName!)
                .Ignore(dest => dest.MiddleName!)
                .Ignore(dest => dest.OrganizationCode!)
                .Ignore(dest => dest.OrganizationName!)
                .Ignore(dest => dest.EmployeeId)
                .Ignore(dest => dest.PositionCode!)
                .Ignore(dest => dest.PositionName!)
                .Ignore(dest => dest.DepartmentCode!)
                .Ignore(dest => dest.DepartmentName!)
                .Ignore(dest => dest.AssignmentsAuthorNavigation!)
                .Ignore(dest => dest.AssignmentsResponsibleExecutorNavigation!)
                .Ignore(dest => dest.AssignmentsResponsibleInspectorNavigation!)
                .Ignore(dest => dest.AssignmentsResponsibleLeaderNavigation!)
                .Ignore(dest => dest.AssignmentHistoryCreatedByNavigation!)
                .Ignore(dest => dest.AssignmentHistoryAddedResponsibleExecutorNavigation!)
                .Ignore(dest => dest.AssignmentHistoryRemovedResponsibleExecutorNavigation!)
                .Ignore(dest => dest.Photo!)
                .Ignore(dest => dest.PhotoS!)
                .Ignore(dest => dest.IsSfrelevant!)
                .Ignore(dest => dest.Occupation!)
                .Ignore(dest => dest.PersonLastModification)
                .Ignore(dest => dest.PositionLastModification)
                .Ignore(dest => dest.DepartmentLastModification)
                .Ignore(dest => dest.OrganizationLastModification)
                .Ignore(dest => dest.AnyLastModification)
                .Ignore(dest => dest.UserRole!)
                .Ignore(dest => dest.RoleId!)
                .Ignore(dest => dest.HireDate!)
                .Ignore(dest => dest.EndDate!)
                .Ignore(dest => dest.AnyLastModification!)
                .Ignore(dest => dest.ProtocolFoldersCreatedByNavigation!)
                .Ignore(dest => dest.ProtocolFoldersUpdatedByNavigation!)
                .Ignore(dest => dest.ProtocolsCreatedByNavigation!)
                .Ignore(dest => dest.ProtocolsUpdatedByNavigation!)
                .Ignore(dest => dest.AssignmentsCreatedByNavigation!)
                .Ignore(dest => dest.AssignmentsUpdatedByNavigation!)
                .Ignore(dest => dest.AssignmentFilesCreatedByNavigation!)
                .Ignore(dest => dest.AssignmentFilesUpdatedByNavigation!)
                .Ignore(dest => dest.CommentsCreatedByNavigation!)
                .Ignore(dest => dest.CommentsUpdatedByNavigation!)
                .Ignore(dest => dest.ProtocolFoldersAllowedEmployeesNavigation!)
                .Ignore(dest => dest.NotificationSettingUserNavigation!)
                .Ignore(dest => dest.IsActive);

            config.NewConfig<Protocol, ProtocolDTO>()
                //заглушка
                .Ignore(dest => dest.Number);

            //RequestDTO

            //Комментарии
            config.NewConfig<CommentRequestDto, Comment>()
                .Map(dest => dest.StatusCreated,
                    src => src.StatusCard)
                .Ignore(dest => dest.Assignment!)
                .Ignore(dest => dest.Id!)
                .Ignore(dest => dest.Created!)
                .Ignore(dest => dest.CreatedBy!)
                .Ignore(dest => dest.CreatedByNavigation!)
                .Ignore(dest => dest.Updated!)
                .Ignore(dest => dest.UpdatedBy!)
                .Ignore(dest => dest.UpdatedByNavigation!)
                .Ignore(dest => dest.Removed!);

            //Поручения
            config.NewConfig<AssignmentShortRequestDto, Assignment>()
                .Map(dest => dest.OrganizationId, src => src.CompanyId)
                .Ignore(dest => dest.Author!)
                .Ignore(dest => dest.AuthorId!)
                .Ignore(dest => dest.Comments!)
                .Ignore(dest => dest.Organization!)
                .Ignore(dest => dest.ExecutorExecutionDate!)
                .Ignore(dest => dest.LeaderExecutionDate!)
                .Ignore(dest => dest.Files!)
                .Ignore(dest => dest.GroupId)
                .Ignore(dest => dest.History!)
                .Ignore(dest => dest.InspectorCheckDate!)
                .Ignore(dest => dest.Name)
                .Ignore(dest => dest.Protocol!)
                .Ignore(dest => dest.Removed!)
                .Ignore(dest => dest.ResponsibleLeader!)
                .Ignore(dest => dest.ResponsibleExecutor!)
                .Ignore(dest => dest.ResponsibleExecutorId!)
                .Ignore(dest => dest.ResponsibleInspector!)
                .Ignore(dest => dest.ResponsibleInspectorId!)
                .Ignore(dest => dest.Status!)
                .Ignore(dest => dest.StatusId!)
                .Ignore(dest => dest.CompletionDate!)
                .Ignore(dest => dest.Subversion)
                .Ignore(dest => dest.Version)
                .Ignore(dest => dest.IsArchived)
                .Ignore(dest => dest.CreatedByNavigation!)
                .Ignore(dest => dest.PeriodicNotifications!)
                .Ignore(dest => dest.UpdatedByNavigation!);

            config.NewConfig<AssignmentRequestDto, Assignment>()
                .Map(dest => dest.AuthorId, src => src.AuthorId ?? src.CreatedBy)
                .Map(dest => dest.OrganizationId, src => src.CompanyId)
                .Ignore(dest => dest.Author!)
                .Ignore(dest => dest.IsArchived)
                .Ignore(dest => dest.Organization!)
                .Ignore(dest => dest.Files!)
                .Ignore(dest => dest.Name)
                .Ignore(dest => dest.Comments!)
                .Ignore(dest => dest.Protocol!) //как будет норм DTO протокола сделать мапинг
                .Ignore(dest => dest.Status!) //заполним отдельно
                .Ignore(dest => dest.ResponsibleLeader!) //заполним отдельно
                .Ignore(dest => dest.ResponsibleExecutor!) //заполним отдельно
                .Ignore(dest => dest.ResponsibleInspector!) //заполним отдельно
                .Ignore(dest => dest.ExecutorExecutionDate!)
                .Ignore(dest => dest.InspectorCheckDate!)
                .Ignore(dest => dest.LeaderExecutionDate!)
                .Ignore(dest => dest.CompletionDate!)
                .Ignore(dest => dest.GroupId)
                .Ignore(dest => dest.History!)
                .Ignore(dest => dest.InspectorCheckDate!)
                .Ignore(dest => dest.Removed!)
                .Ignore(dest => dest.ResponsibleExecutorId!)//заполним отдельно
                .Ignore(dest => dest.ResponsibleInspectorId!)//заполним отдельно
                .Ignore(dest => dest.ResponsibleLeaderId!)//заполним отдельно
                .Ignore(dest => dest.UpdatedByNavigation!)
                .Ignore(dest => dest.CreatedByNavigation!)
                .Ignore(dest => dest.PeriodicNotifications!);

            //Протоколы
            config.NewConfig<CreateProtocolRequestDTO, Protocol>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.UpdatedBy!)
                .Ignore(dest => dest.Removed!)
                .Ignore(dest => dest.Assignments)
                .Ignore(dest => dest.Folder)
                .Ignore(dest => dest.Name)
                .Ignore(dest => dest.IsArchived)
                .Ignore(dest => dest.CreatedByNavigation!)
                .Ignore(dest => dest.UpdatedByNavigation!)
                .Ignore(dest => dest.Updated!)
                .Ignore(dest => dest.Removed!);

            //ResponseDTO

            //Организации
            config.NewConfig<Organization, OrganizationResponseDto>()
                .Map(dest => dest.Id, src => src.OrganizationId);

            //Комментарии
            config.NewConfig<Comment, CommentResponseDto>()
                .Map(dest => dest.CreatedBy,
                    src => src.CreatedByNavigation.PositionId)
                .Map(
                    dest => dest.AuthorFullName,
                    src =>
                        $"{src.CreatedByNavigation!.LastName!} "
                            + $"{src.CreatedByNavigation!.FirstName!} "
                            + $"{src.CreatedByNavigation!.MiddleName!}")
                .Ignore(x => x.UserCanRemoveComment);

            //Папки
            config.NewConfig<Services.DAO.ProtocolFolder, RemoveFolderResponseDto>();
            config.NewConfig<Services.DAO.ProtocolFolder, Models.DTO.Response.ProtocolFolder>()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.SurnameInitials
                    , src => src.CreatedByNavigation == null ? "Администратор папки не установлен" : src.CreatedByNavigation.GetInitials())
                .Map(dest => dest.AllowedEmployees, src
                    => src.AllowedEmployeesNavigation ?? new List<Employee>())
                .Map(dest => dest.FullName, src => src.CreatedByNavigation == null ? "Администратор папки не установлен" : src.CreatedByNavigation.GetFormatedName());

            //Файлы поручений
            config.NewConfig<AssignmentFile, UploadFileToDbResponseDto>();
            config.NewConfig<AssignmentFile, RemoveFileDtoResponse>();
            config.NewConfig<AssignmentFile, AssignmentFileResponseDto>()
                .Map(dest => dest.Name,
                    src => src.OriginName)
                .Map(dest => dest.CreatedBy,
                    src => src.CreatedByNavigation);
            config.NewConfig<AssignmentFile, DownloadFileResponse>();

            config.NewConfig<InstructuonFile, UploadFileToDbResponseDto>();
            //config.NewConfig<InstructuonFile, RemoveFileDtoResponse>();
            config.NewConfig<InstructuonFile, AssignmentFileResponseDto>()
                .Ignore(x => x.Created)
                .Ignore(x => x.CreatedBy)
                .Map(dest => dest.Name,
                    src => src.OriginName);

            config.NewConfig<InstructuonFile, FileNameResponseDto>();

            config.NewConfig<InstructuonFile, DownloadFileResponse>()
                .Ignore(x => x.Created)
                .Ignore(x => x.CreatedBy)
                .Ignore(x => x.Updated)
                .Ignore(x => x.UpdatedBy);

            //Статусы
            config.NewConfig<AssignmentStatus, AssignmentStatusResponse>();

            //Протоколы

            config.NewConfig<Protocol, CreateProtocolResponseDTO>();
            config.NewConfig<Protocol, ProtocolResponseDTO>()
                .Map(dest => dest.CreatorShortName,
                    src => src.CreatedByNavigation!.GetInitials());
            config.NewConfig<Assignment, ShortProtocolReportResponseDto>()
                .Map(dest => dest.Status, src => src.Status!.Name)
                .Map(dest => dest.Comment,
                    src => src.Comments.IsNullOrEmpty()
                        ? src.Status!.Name
                        : src.Comments!.Last().Content);

            //Сотрудник
            config.NewConfig<(Employee?, string), ResponsibleEmployee>()
                .Map(dest => dest.EmployeeName,
                    src => GetEmployeeInitials(src))
                .Map(dest => dest.Position,
                    src => src.Item2);

            //Поручения
            config.NewConfig<Assignment, AssignmentResponseShort>()
                .Map(dest => dest.Company,
                    src => src.Organization)
                .Map(dest => dest.ResponsibleEmployees,
                    src => GetResponsibleEmployees(src))
                .Map(dest => dest.Name,
                    src => BuildShortAssignmentViewName(src.Name))
                .Map(dest => dest.Status,
                    src => src.Status!.Id)
                .Map(dest => dest.ProtocolInfo,
                    src => BuildProtocolInfoString(src.Protocol!))
                .Map(dest => dest.ExecutionDate,
                    src => src.ExecutionDate)
                .Map(dest => dest.Comment,
                    src => src.Comments.IsNullOrEmpty()
                        ? string.Empty
                        : src.Comments!.First().Content);

            config.NewConfig<Assignment, AssignmentResponse>()
                .Map(dest => dest.AllowedEmployeesNavigation,
                    src => src.Protocol.Folder.AllowedEmployeesNavigation)
                .Map(dest => dest.ProtocolCreatedBy,
                    src => src.Protocol.CreatedBy)
                .Map(dest => dest.FolderCreatedBy,
                    src => src.Protocol.Folder.CreatedBy)
                .Map(dest => dest.Company,
                    src => src.Organization)
                .Map(dest => dest.EventDirection,
                    src => src.Protocol!.Folder.Name!)
                .Map(dest => dest.Name,
                    src => BuildLongAssignmentViewName(src.Name))
                .Map(dest => dest.Status,
                    src => src.Status!.Id)
                .Map(dest => dest.Files,
                    src => src.Files)
                .Map(dest => dest.ProtocolInfo,
                    src => BuildProtocolInfoString(src.Protocol!))
                .Ignore(dest => dest.ResponsibleExecutors!)
                .Ignore(dest => dest.ResponsibleInspectors!)
                .Ignore(dest => dest.ResponsibleLeaders!)
                .Ignore(dest => dest.UserCanAddComment);

            //Дерево версий поручения
            config.NewConfig<Assignment, AssignmentVersionResponse>()
                .Map(dest => dest.CurrentStatus,
                    src => src.Status!.Name ?? "Статус не доступен");

            //Настройки уведомлений пользователя
            config.NewConfig<NotificationSetting, NotificationSettingResponseDTO>()
                .Map(dest => dest.NewTitle, src => "Новое поручение")
                .Map(dest => dest.WeeklyTitle, src => "Еженедельное напоминание")
                .Map(dest => dest.StatusChangeTitle, src => "Изменение статуса")
                .Map(dest => dest.UserEmail, src => src.UserNavigation.Email);

            //Уведомления
            config.NewConfig<NotificationSettingRequestDTO, NotificationSetting>()
                .Ignore(dest => dest.UserPositionId)
                .Ignore(dest => dest.UserNavigation)
                .Ignore(dest => dest.Id);

            config.NewConfig<PeriodicNotification, PeriodicNotificationResponseDto>()
                .Map(dest => dest.Recipient, src => src.Recipient!.Email)
                .Map(dest => dest.NotificationType, src => src.NotificationType);

            return config;
        }

        private static string? GetEmployeeInitials((Employee?, string) src)
        {
            return src.Item1?.GetInitials();
        }

        /// <summary>
        /// Собрать строку для отображения информации о протоколе для поручения
        /// </summary>
        /// <param name="protocol">Протокол в рамках которого создано поручение</param>
        private static string BuildProtocolInfoString(Protocol protocol)
        {
            return $"{protocol.Name}";
            //+ $"{protocol.Created.ToShortDateString()} "
            //+ $"{protocol.Folder.Name}";
        }

        /// <summary>
        /// Собрать короткое имя поручения для отображения
        /// </summary>
        /// <param name="name">Имя поручения из БД</param>
        private static string BuildShortAssignmentViewName(string name)
        {
            const string prefix = "П-";
            return prefix + name;
        }

        /// <summary>
        /// Собрать полное имя поручения для отображения
        /// </summary>
        /// <param name="name">Имя поручения из БД</param>
        private static string BuildLongAssignmentViewName(string name)
        {
            const string prefix = "Поручение-";
            return prefix + name;
        }

        private static List<(Employee?, string)> GetResponsibleEmployees(Assignment assignment)
        {
            return new List<(Employee?, string)>
                    {
                        new (assignment.Author, ResponsiblePositions.Author),
                        new (assignment.ResponsibleLeader, ResponsiblePositions.ResponsibleLeader),
                        new (assignment.ResponsibleExecutor, ResponsiblePositions.ResponsibleExecutor),
                        new (assignment.ResponsibleInspector, ResponsiblePositions.ResponsibleInspector)
                    }
                    .ToList();
        }
    }
}