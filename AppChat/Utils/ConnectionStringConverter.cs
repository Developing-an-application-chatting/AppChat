namespace AppChat.Utils
{
    public static class ConnectionStringConverter
    {
        public static string ConvertConnectionString(string databaseUrl)
        {
            var uri = new Uri(databaseUrl);

            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo[1];

            var host = uri.Host;
            var port = uri.Port;
            var database = uri.AbsolutePath.TrimStart('/');

            return $"Host={host};" +
                   $"Port={port};" +
                   $"Username={username};" +
                   $"Password={password};" +
                   $"Database={database};" +
                   $"SSL Mode=Require;Trust Server Certificate=true";
        }
    }
}
