using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using EventsAndAssignments_DataTransfer.Services;
using EventsAndAssignments_DataTransfer.DAO.MIMPublish2Db;
using System.Text.Encodings.Web;
using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole(o =>
{
    o.JsonWriterOptions = new JsonWriterOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Indented = true
    };
    o.TimestampFormat = "dd.MM.yyyy HH:mm:ss.fff";
});

// Add services to the container.

builder.Configuration
    .AddJsonFile("appsettings.data-transfer.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(so =>
{
    so.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DataTransfer Service",
        Description =
            "Сервис переноса (репликации) данных между БД 'MSK-SCL-003.msk.evraz.com.MIMPublish2' и "
                + "'RUK-SQL-XP067.PRJ10252'. Запускается по расписанию из appsettings.json. Через API возможно"
                + " включить/выключить сервис и проверить логи работы сервиса.",
        Contact = new OpenApiContact { Email = "Artem.Kostylev@evraz.com" }
    });
    so.IncludeXmlComments("doc.xml");
});

builder.Services.AddSingleton<ConnectionStringService>();

builder.Services.AddDbContext<MIMPublish2Context, MIMPublish2Context>(
    (s, o) => o.UseSqlServer(
        s.GetRequiredService<ConnectionStringService>().MIMPublish2ConnectionString),
        ServiceLifetime.Singleton);
builder.Services.AddDbContext<EventsAndAssignmentsContext, EventsAndAssignmentsContext>(
    (s, o) => o.UseSqlServer(
        s.GetRequiredService<ConnectionStringService>().EventsAndAssignmentsConnectionString),
        ServiceLifetime.Singleton);

builder.Services.AddSingleton<DbTransferServiceControl>();
builder.Services.AddHostedService<DbTransferService>();

WebApplication app = builder.Build();

ILogger<Program> logger = app.Services.GetService<ILogger<Program>>()!;
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

app.UsePathBase(pathPrefix);
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

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
