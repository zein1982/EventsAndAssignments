namespace EventsAndAssignments_DataTransfer.Services
{
    /// <summary>
    /// Генерирует и хранит строки подключение к базам данных, с которыми работает приложение
    /// </summary>
    public class ConnectionStringService
    {
        private bool _connectionStringFromVariablesIsUsed;

        public ConnectionStringService(IConfiguration configuration, ILogger<ConnectionStringService> logger)
        {
            EventsAndAssignmentsConnectionString = null!;
            MIMPublish2ConnectionString = null!;
            GetConnectionString(configuration);
            LogConnectionStringCondition(logger);
        }

        /// <summary>
        /// Строка подключения к БД EventsAndAssignments
        /// </summary>
        public string EventsAndAssignmentsConnectionString { get; private set; }

        /// <summary>
        /// Строка подключения к MIMPublish2
        /// </summary>
        public string MIMPublish2ConnectionString { get; private set; }

        /// <summary>
        /// Сгенерировать строки подключения из переменных окружения, или получить их из файла конфигурации
        /// </summary>
        private void GetConnectionString(IConfiguration configuration)
        {
            string? databaseHost = configuration["DATABASE_HOST"];
            string? databaseName = configuration["DATABASE_NAME"];
            string? databaseBackendUsername = configuration["DATABASE_BACKEND_USERNAME"];
            string? databaseBackendPassword = configuration["DATEBASE_BACKEND_PASSWORD"];
            string? databaseMimHost = configuration["DATABASE_MIM_HOST"];
            string? databaseMimName = configuration["DATABASE_MIM_NAME"];
            string? databaseMimUsername = configuration["DATABASE_MIM_USERNAME"];
            string? databaseMimPassword = configuration["DATABASE_MIM_PASSWORD"];

            if (databaseHost is not null
                && databaseName is not null
                && databaseBackendUsername is not null
                && databaseBackendPassword is not null
                && databaseMimHost is not null
                && databaseMimName is not null
                && databaseMimUsername is not null
                && databaseMimPassword is not null)
            {
                EventsAndAssignmentsConnectionString = $"Data Source={databaseHost};Initial Catalog={databaseName};"
                    + $"User ID={databaseBackendUsername};Password={databaseBackendPassword};Trusted_Connection=True;Encrypt=False";
                //mimConnectionString = $"Data Source={databaseMimHost};Initial Catalog={databaseMimName};"
                //    + $"User ID={databaseMimUsername};Password={databaseMimPassword};Trusted_Connection=True;Encrypt=False;";
                MIMPublish2ConnectionString = $"Server={databaseMimHost};Database={databaseMimName};User ID={databaseMimUsername};Password={databaseMimPassword};"
                    + "Integrated Security=True;Encrypt=False";

                _connectionStringFromVariablesIsUsed = true;
            }
            else
            {
                EventsAndAssignmentsConnectionString = configuration.GetConnectionString("EventsAndAssignments")!;
                MIMPublish2ConnectionString = configuration.GetConnectionString("MIMPublish2")!;
            }
        }

        /// <summary>
        /// Залоггировать состояние строк подключения
        /// </summary>
        private void LogConnectionStringCondition(ILogger<ConnectionStringService> logger)
        {
            logger.LogWarning(_connectionStringFromVariablesIsUsed
                ? "Строки подключения сгенерированы из переменных окружения"
                : "Строки подключения взяты из appsettings.data-transfer.json");

            string eventsAndAssignmentsConnStrWithoutPassword = EventsAndAssignmentsConnectionString.Contains("Password")
                ? EventsAndAssignmentsConnectionString.Remove(EventsAndAssignmentsConnectionString.IndexOf("Password"))
                : EventsAndAssignmentsConnectionString;
            string mimConnStrWithoutPassword = MIMPublish2ConnectionString.Contains("Password")
                ? MIMPublish2ConnectionString.Remove(MIMPublish2ConnectionString.IndexOf("Password")) : MIMPublish2ConnectionString;
                        logger.LogInformation("Строки подключения (без паролей):"
                                        + Environment.NewLine
                                        + eventsAndAssignmentsConnStrWithoutPassword
                                        + Environment.NewLine
                                        + mimConnStrWithoutPassword);
        }
    }
}
