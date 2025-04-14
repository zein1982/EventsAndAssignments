using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Json;
using EventsAndAssignments.Api.Configs;
using EventsAndAssignments.API.Configs;
using EventsAndAssignments.API.Quartz.Jobs;
using EventsAndAssignments.Db;
using EventsAndAssignments.Services.Options;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Quartz;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddEnvironmentVariables(prefix: "SECRET_");
using ILoggerFactory loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.AddJsonConsole(o => o.JsonWriterOptions = new JsonWriterOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Indented = true
    });
});
ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Program started...");

builder.Logging.AddJsonConsole(o => o.JsonWriterOptions = new JsonWriterOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    Indented = true
});

string? connectionString = ConnectionStringBuilder.GetDefaultConnectionString(builder.Configuration);
string? hseConnectionString = ConnectionStringBuilder.GetHseConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<HseDbContext>(options => options.UseSqlServer(hseConnectionString));

logger.LogInformation(ConnectionStringBuilder.ConnectionStringFromVars
    ? "Connection string building from ENV VARS"
    : "Connection string building from appsettings.json");

// Add services to the container.

//Привязка настроек
builder.Services.Configure<NotificationsOptions>(builder.Configuration.GetSection(NotificationsOptions.Notifications));
builder.Services.Configure<MailOptions>(builder.Configuration.GetSection(MailOptions.Mail));

//Глобальная обработка ошибок
builder.Services.AddProblemDetails();

//Планировщик задач
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    JobKey jobKey = new("Send notifications");
    q.AddJob<SendNotificationsJob>(options =>
        options.WithIdentity(jobKey));

    string cronExpression = builder.Configuration["QuartzJobs:SendNotificationsJob:CronExpression"]
        ?? throw new ConfigurationErrorsException(
            "Configuration element [QuartzJobs:DbfParserJob:CronExpression] not valid or not specified");

    logger.LogInformation("Set new cron expression for Job {Job} with Cron Expression: {CronExpression}", jobKey.Name, cronExpression);
    q.AddTrigger(options => options
        .ForJob(jobKey)
        .WithIdentity(jobKey.Name + " trigger")
        .WithCronSchedule(cronExpression));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//builder.Services.AddControllers(o => o.AllowEmptyInputInBodyModelBinding = true);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

KeycloakConfig.Init(builder);

SwaggerConfig.Init(builder);

logger.LogInformation("Текущий Keycloak Id: {KeyCloakId}", KeycloakConfig.KeycloakClientId);
logger.LogInformation("Текущий Keycloak URL: {KeyCloakUrl}", KeycloakConfig.KeycloakUrlWithRealm);

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(
            JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

DependencyInjectionConfig.Configure(builder);

WebApplication app = builder.Build();

IConfiguration? config = app.Services.GetService<IConfiguration>();

// Настройка URL Prefix для ендпоинтов и swagger-ui
string? useUrlPrefix = config?["UseURLPrefix"];
string pathPrefix = string.Empty;

if (string.Equals(useUrlPrefix, "True", StringComparison.CurrentCultureIgnoreCase))
{
    pathPrefix = config?["UrlPathPrefix"] ?? throw new NullReferenceException("UrlPathPrefix");
    if (!string.IsNullOrEmpty(pathPrefix) && !pathPrefix.StartsWith('/'))
    {
        pathPrefix = pathPrefix.Insert(0, "/");
    }
}

app.UseProblemDetails();

app.UsePathBase(pathPrefix);
app.UseRouting();

//Проверка маппинга в MappingConfig
Mapster.TypeAdapterConfig mappingConfig = MappingConfig.GetConfig();
mappingConfig.RequireExplicitMapping = true;
mappingConfig.RequireDestinationMemberSource = true;
mappingConfig.Compile();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //builder.Configuration.AddConfiguration("")
}

if (app.Environment.IsStaging())
{
}

if (app.Environment.IsProduction())
{
}

app.UseSwagger();
app.UseSwaggerUI(o => o.OAuthClientId(KeycloakConfig.KeycloakClientId));

// Редирект на страницу swagger-ui
app.Use(async (context, next) =>
{
    await next.Invoke();
    PathString requestPath = context.Request.Path;
    if (requestPath == "/" || requestPath == "" || requestPath == pathPrefix || requestPath == $"{pathPrefix}/")
    {
        context.Response.Redirect($"{pathPrefix}/swagger");
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
