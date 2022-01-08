using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Data
{
    public class ConnectionService
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            //Gather connection string defined in appsettings.json
            var connectionString = configuration.GetConnectionString("PostgresConnection");
            //Gather connection string from Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            //return Heroku connection string if it's present; otherwise return local connection string
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }
        private static string BuildConnectionString(string databaseUrl)
        {
            //Uniform Resource Identifier
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(":");

            //Define new connection string using uri components
            return new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true

            }.ToString();
        }
    }
}
