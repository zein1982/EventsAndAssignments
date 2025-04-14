using EventsAndAssignments.API.Authentication;
using EventsAndAssignments.API.Authentication.Assignments;
using EventsAndAssignments.Db.Repositories;
using EventsAndAssignments.Infrastructure;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.Data;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;

namespace EventsAndAssignments.API.Configs
{
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Настройка DI
        /// </summary>
        public static void Configure(WebApplicationBuilder builder)
        {
            //string connection = builder.Configuration.GetConnectionString("DBConn");

            //DI для маппинга
            builder.Services.AddSingleton(MappingConfig.GetConfig());
            builder.Services.AddScoped<IMapper, ServiceMapper>();

            //DI для сервисов 
            builder.Services.AddTransient<IReportService, ReportService>();
            builder.Services.AddTransient<IFilterService, FilterService>();
            builder.Services.AddTransient<IProtocolFoldersService, ProtocolFoldersService>();
            builder.Services.AddTransient<IAssignmentsService, AssignmentsService>();
            builder.Services.AddTransient<IProtocolService, ProtocolService>();
            builder.Services.AddTransient<IAssignmentHistoryService, AssignmentHistoryService>();
            builder.Services.AddTransient<ICommentService, CommentService>();
            builder.Services.AddTransient<IAssignmentHistoryMessageBuilderService, AssignmentHistoryMessageBuilderService>();
            builder.Services.AddTransient<IFileService, FileService>();
            builder.Services.AddTransient<IAssignmentCountService, AssignmentCountService>();
            builder.Services.AddTransient<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IDataSeedService, DataSeedService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IFileInstructionService, FileInstructionService>();
            builder.Services.AddScoped<ICalendarService, CalendarService>();

            //DI DB
            builder.Services.AddScoped<ICommentGateway, CommentGateway>();
            builder.Services.AddScoped<IProtocolFoldersGateway, ProtocolFoldersGateway>();
            builder.Services.AddScoped<IAssignmentsGateway, AssignmentsGateway>();
            builder.Services.AddScoped<IProtocolGateway, ProtocolGateway>();
            builder.Services.AddScoped<IAssignmentHistoryGateway, AssignmentHistoryGateway>();
            builder.Services.AddScoped<IFileGateway, FileGateway>();
            builder.Services.AddScoped<IOrganizationGateway, OrganizationGateway>();
            builder.Services.AddScoped<IEmployeeGateway, EmployeeGateway>();
            builder.Services.AddScoped<IFilterGateway, FilterGateway>();
            builder.Services.AddScoped<IPermissionGateway, PermissionGateway>();
            builder.Services.AddScoped<IDataSeedGateway, DataSeedGateway>();
            builder.Services.AddScoped<INotificationGateway, NotificationGateway>();
            builder.Services.AddScoped<IInstructionFileGateWay, InstructionFileGateWay>();
            builder.Services.AddScoped<ICalendarGateway, CalendarGateway>();

            //DI Infrustructure
            //builder.Services.AddTransient<IEmailSender, FakeEmailSender>();
            builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();

            //builder.Services.AddScoped<IAuthorizationHandler, EmployeeCanAddCommentHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, EmployeeIsInAssignmentHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            //Настройка сервиса сотрудников
            //builder.Services.AddScoped<IEmployeeService>(x =>
            //new EmployeeService(x.GetRequiredService<ILogger<EmployeeService>>(), "Login@evraz.com"));
        }
    }
}