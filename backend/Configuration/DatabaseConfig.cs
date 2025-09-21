namespace ProjectManagement.Api.Configuration
{
    public class DatabaseConfig
    {
        public const string AzureServer = "upt-dbs-996.database.windows.net";
        public const string DatabaseName = "shorten";
        public const string DefaultUser = "adminuser";
        
        public static string GetConnectionString(string? password = null)
        {
            var actualPassword = password ?? "Admin123456";
            return $"Server={AzureServer};Database={DatabaseName};User Id={DefaultUser};Password={actualPassword};TrustServerCertificate=true;MultipleActiveResultSets=true;";
        }
        
        public static string GetConnectionStringFromEnvironment()
        {
            var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? AzureServer;
            var database = Environment.GetEnvironmentVariable("DB_NAME") ?? DatabaseName;
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? DefaultUser;
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "Admin123456";
            
            return $"Server={server};Database={database};User Id={user};Password={password};TrustServerCertificate=true;MultipleActiveResultSets=true;";
        }
    }
}