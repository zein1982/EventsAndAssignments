using EventsAndAssignments.Services.Extensions;

namespace EventsAndAssignments.Api.Configs
{
    public static class ConnectionStringBuilder
    {
        public static bool ConnectionStringFromVars;

        /// <summary>
        /// Вычисляет стандартную строку подключения
        /// </summary>
        public static string? GetDefaultConnectionString(IConfiguration configuration)
        {
            string? dbHost = configuration["DATABASE_HOST"];
            string? dbName = configuration["DATABASE_NAME"];
            string? dbUserName = configuration["DATABASE_BACKEND_USERNAME"];
            string? dbUserPassword = configuration["DATEBASE_BACKEND_PASSWORD"];

            string? connectionString = configuration.GetConnectionString("DBConn");

            if (dbHost.HasValue() && dbName.HasValue() && dbUserName.HasValue() && dbUserPassword.HasValue())
            {
                connectionString = $"Server={dbHost};Database={dbName};User ID={dbUserName};Password={dbUserPassword};"
                    + "Trusted_Connection=True;Encrypt=False";
                ConnectionStringFromVars = true;
            }

            return connectionString;
        }

        /// <summary>
        /// Вычисляет стандартную строку подключения
        /// </summary>
        public static string? GetHseConnectionString(IConfiguration configuration)
        {
            const string? dbHost = "EVRAZ-SQL-HSE.sib.evraz.com\\HSE";
            const string? dbName = "HSE-Inspection";
            string? dbUserName = configuration["DATABASE_BACKEND_USERNAME"];
            string? dbUserPassword = configuration["DATEBASE_BACKEND_PASSWORD"];

            //Для локальной разработки
            string? hseUser = configuration.GetValue<string>("DB:HSE:USERNAME");
            string? hsePass = configuration.GetValue<string>("DB:HSE:PASSWORD");
            string? connectionString = $"Server={dbHost};Database={dbName};"
                + $"User ID={hseUser};Password={hsePass};"
                + "Trusted_Connection=True;Encrypt=False";

            //Для ландшафтов
            if (dbHost.HasValue() && dbName.HasValue() && dbUserName.HasValue() && dbUserPassword.HasValue())
            {
                connectionString = $"Server={dbHost};Database={dbName};User ID={dbUserName};Password={dbUserPassword};"
                    + "Trusted_Connection=True;Encrypt=False";
            }

            return connectionString;
        }

        /// <summary>
        /// Строка подключения для миграций на тест.
        /// </summary>
        public static string? GetConnectionStringForMigration(IConfiguration configuration)
        {
            string? dbHost = configuration["DATABASE_HOST"];
            string? dbName = configuration["DATABASE_NAME"];
            string? dbMigrationUserName = configuration["DATABASE_MIGRATE_USERNAME"];
            string? dbMigrationUserPassword = configuration["DATEBASE_MIGRATE_PASSWORD"];

            //собираем строку
            string? connectionString = configuration.GetConnectionString("DBConn");

            if (dbHost.HasValue() && dbName.HasValue() && dbMigrationUserName.HasValue() && dbMigrationUserPassword.HasValue())
            {
                connectionString = $"Server={dbHost};Database={dbName};User ID={dbMigrationUserName};Password={dbMigrationUserPassword};"
                    + "Trusted_Connection=True;Encrypt=False";
                ConnectionStringFromVars = true;
            }

            return connectionString;
        }
    }
}