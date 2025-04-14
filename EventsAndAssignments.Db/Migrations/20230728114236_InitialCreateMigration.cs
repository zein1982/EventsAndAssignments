using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventsAndAssignments.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignmentStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    IsInShortLine = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_AssignmentStatuses", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Permissions", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Roles", x => x.Id));

            migrationBuilder.CreateTable(
                name: "PuplicEmployeeViews",
                columns: table => new
                {
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TabelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PhotoS = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IsSfrelevant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<long>(type: "bigint", nullable: true),
                    PersonLastModfication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PositionLastModfication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentLastModfication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganizationLastModfication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HireDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnyLastModfication = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuplicEmployeeViews", x => x.PositionId);
                    table.ForeignKey(
                        name: "FK_PuplicEmployeeViews_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Companies_PuplicEmployeeViews_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    IsWeekly = table.Column<bool>(type: "bit", nullable: false),
                    IsStatusChange = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_PuplicEmployeeViews_UserPositionId",
                        column: x => x.UserPositionId,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProtocolFolders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtocolFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProtocolFolders_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProtocolFolders_PuplicEmployeeViews_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Protocols",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    FolderId = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Protocols_ProtocolFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "ProtocolFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Protocols_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Protocols_PuplicEmployeeViews_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaderExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutorExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectorCheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subversion = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    ProtocolId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsibleLeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsibleExecutorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponsibleInspectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_AssignmentStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "AssignmentStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assignments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assignments_Protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "Protocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_PuplicEmployeeViews_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId");
                    table.ForeignKey(
                        name: "FK_Assignments_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignments_PuplicEmployeeViews_ResponsibleExecutorId",
                        column: x => x.ResponsibleExecutorId,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId");
                    table.ForeignKey(
                        name: "FK_Assignments_PuplicEmployeeViews_ResponsibleInspectorId",
                        column: x => x.ResponsibleInspectorId,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId");
                    table.ForeignKey(
                        name: "FK_Assignments_PuplicEmployeeViews_ResponsibleLeaderId",
                        column: x => x.ResponsibleLeaderId,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignments_PuplicEmployeeViews_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignmentId = table.Column<long>(type: "bigint", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_PuplicEmployeeViews_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SafetyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AssignmentId = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Files_PuplicEmployeeViews_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodicNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Template = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<long>(type: "bigint", nullable: true),
                    ProtocolId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodicNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodicNotifications_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PeriodicNotifications_Protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "Protocols",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PeriodicNotifications_PuplicEmployeeViews_RecipientPositionId",
                        column: x => x.RecipientPositionId,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<long>(type: "bigint", nullable: true),
                    ModificationType = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddedResponsibleExecutor = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RemovedResponsibleExecutor = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromStatus = table.Column<long>(type: "bigint", nullable: true),
                    ToStatus = table.Column<long>(type: "bigint", nullable: true),
                    AddedFile = table.Column<long>(type: "bigint", nullable: true),
                    RemovedFile = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_AssignmentStatuses_FromStatus",
                        column: x => x.FromStatus,
                        principalTable: "AssignmentStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_AssignmentStatuses_ToStatus",
                        column: x => x.ToStatus,
                        principalTable: "AssignmentStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_Files_AddedFile",
                        column: x => x.AddedFile,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_Files_RemovedFile",
                        column: x => x.RemovedFile,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_PuplicEmployeeViews_AddedResponsibleExecutor",
                        column: x => x.AddedResponsibleExecutor,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId");
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId");
                    table.ForeignKey(
                        name: "FK_AssignmentHistories_PuplicEmployeeViews_RemovedResponsibleExecutor",
                        column: x => x.RemovedResponsibleExecutor,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodicNotificationId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_PeriodicNotifications_PeriodicNotificationId",
                        column: x => x.PeriodicNotificationId,
                        principalTable: "PeriodicNotifications",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "CreateFolder" },
                    { 2L, "ReadFolder" },
                    { 3L, "UpdateFolder" },
                    { 4L, "RemoveFolder" },
                    { 5L, "EmployeeIsInAssignment" },
                    { 6L, "CreateProtocol" },
                    { 7L, "ReadProtocol" },
                    { 8L, "UpdateProtocol" },
                    { 9L, "RemoveProtocol" },
                    { 10L, "RemoveAssignment" },
                    { 11L, "RemoveFile" },
                    { 12L, "CreateAssignment" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "SystemAdmin" },
                    { 2L, "Admin" },
                    { 3L, "SimpleUser" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1L, 1L },
                    { 2L, 1L },
                    { 3L, 1L },
                    { 4L, 1L },
                    { 6L, 1L },
                    { 7L, 1L },
                    { 8L, 1L },
                    { 9L, 1L },
                    { 10L, 1L },
                    { 12L, 1L },
                    { 1L, 2L },
                    { 2L, 2L },
                    { 3L, 2L },
                    { 4L, 2L },
                    { 6L, 2L },
                    { 7L, 2L },
                    { 8L, 2L },
                    { 9L, 2L },
                    { 10L, 2L },
                    { 12L, 2L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_AddedFile",
                table: "AssignmentHistories",
                column: "AddedFile");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_AddedResponsibleExecutor",
                table: "AssignmentHistories",
                column: "AddedResponsibleExecutor");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_AssignmentId",
                table: "AssignmentHistories",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_CreatedBy",
                table: "AssignmentHistories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_FromStatus",
                table: "AssignmentHistories",
                column: "FromStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_RemovedFile",
                table: "AssignmentHistories",
                column: "RemovedFile");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_RemovedResponsibleExecutor",
                table: "AssignmentHistories",
                column: "RemovedResponsibleExecutor");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentHistories_ToStatus",
                table: "AssignmentHistories",
                column: "ToStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AuthorId",
                table: "Assignments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CompanyId",
                table: "Assignments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CreatedBy",
                table: "Assignments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ProtocolId",
                table: "Assignments",
                column: "ProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ResponsibleExecutorId",
                table: "Assignments",
                column: "ResponsibleExecutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ResponsibleInspectorId",
                table: "Assignments",
                column: "ResponsibleInspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ResponsibleLeaderId",
                table: "Assignments",
                column: "ResponsibleLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_StatusId",
                table: "Assignments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_UpdatedBy",
                table: "Assignments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AssignmentId",
                table: "Comments",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedBy",
                table: "Comments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UpdatedBy",
                table: "Comments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CreatedBy",
                table: "Companies",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UpdatedBy",
                table: "Companies",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Files_AssignmentId",
                table: "Files",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CreatedBy",
                table: "Files",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Files_UpdatedBy",
                table: "Files",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PeriodicNotificationId",
                table: "Notifications",
                column: "PeriodicNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_UserPositionId",
                table: "NotificationSettings",
                column: "UserPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodicNotifications_AssignmentId",
                table: "PeriodicNotifications",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodicNotifications_ProtocolId",
                table: "PeriodicNotifications",
                column: "ProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodicNotifications_RecipientPositionId",
                table: "PeriodicNotifications",
                column: "RecipientPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolFolders_CreatedBy",
                table: "ProtocolFolders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolFolders_UpdatedBy",
                table: "ProtocolFolders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_CreatedBy",
                table: "Protocols",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_FolderId",
                table: "Protocols",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_UpdatedBy",
                table: "Protocols",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PuplicEmployeeViews_RoleId",
                table: "PuplicEmployeeViews",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentHistories");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "PeriodicNotifications");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "AssignmentStatuses");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Protocols");

            migrationBuilder.DropTable(
                name: "ProtocolFolders");

            migrationBuilder.DropTable(
                name: "PuplicEmployeeViews");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}