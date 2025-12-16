using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using WebAPI.Models;
using System.Configuration;


namespace WebAPI.Helpers
{
    public class ApplicationSQLHelper
    {
        private static readonly string ConnectionString =
         System.Configuration.ConfigurationManager
        .ConnectionStrings["SomiodConnectionString"]
        .ConnectionString;

        public static List<Application> GetApplications()
        {
            var list = new List<Application>();
            const string query = "SELECT Id, ResourceName, CreationDateTime FROM Applications";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(LoadApplication(reader));
                }
            }

            return list;
        }

        public static Application GetApplication(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return null;

            const string query =
                "SELECT Id, ResourceName, CreationDateTime " +
                "FROM Applications WHERE ResourceName = @rn";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@rn", resourceName);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? LoadApplication(reader) : null;
                }
            }
        }

        public static int GetLastId()
        {
            const string query = "SELECT ISNULL(MAX(Id), 0) FROM Applications";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static Application CreateApplication(Application app)
        {
            if (app == null) return null;

            if (string.IsNullOrWhiteSpace(app.ResourceName))
                app.ResourceName = $"app{GetLastId() + 1}";

            if (app.CreationDateTime == default)
                app.CreationDateTime = DateTime.UtcNow;

            const string insert =
                "INSERT INTO Applications (ResourceName, CreationDateTime) " +
                "VALUES (@rn, @dt)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@rn", app.ResourceName);
                cmd.Parameters.AddWithValue("@dt", app.CreationDateTime);
                conn.Open();

                return cmd.ExecuteNonQuery() > 0
                    ? GetApplication(app.ResourceName)
                    : null;
            }
        }

        public static Application UpdateApplication(string oldResourceName, Application app)
        {
            if (string.IsNullOrWhiteSpace(oldResourceName) || app == null)
                return null;

            const string update =
                "UPDATE Applications SET ResourceName = @newName " +
                "WHERE ResourceName = @oldName";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(update, conn))
            {
                cmd.Parameters.AddWithValue("@newName", app.ResourceName);
                cmd.Parameters.AddWithValue("@oldName", oldResourceName);
                conn.Open();

                return cmd.ExecuteNonQuery() > 0
                    ? GetApplication(app.ResourceName)
                    : null;
            }
        }

        public static Application DeleteApplication(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return null;

            var existing = GetApplication(resourceName);
            if (existing == null) return null;

            const string delete =
                "DELETE FROM Applications WHERE ResourceName = @rn";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(delete, conn))
            {
                cmd.Parameters.AddWithValue("@rn", resourceName);
                conn.Open();

                return cmd.ExecuteNonQuery() > 0 ? existing : null;
            }
        }

        private static Application LoadApplication(SqlDataReader reader)
        {
            return new Application
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ResourceName = reader.GetString(reader.GetOrdinal("ResourceName")),
                CreationDateTime = reader.GetDateTime(reader.GetOrdinal("CreationDateTime"))
            };
        }
    }
}
