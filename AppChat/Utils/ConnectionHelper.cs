using Npgsql;

namespace AppChat.Utils
{
    public static class ConnectionHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                Console.WriteLine($"Using DATABASE_URL from environment: {databaseUrl}");
                if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
                {
                    return BuildConnectionString(databaseUrl);
                }
                return databaseUrl;
            }

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"Using DefaultConnection from appsettings: {connectionString}");
                return connectionString;
            }

            throw new InvalidOperationException("Connection string not found!");
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }
    }
}
