using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using WebAPI.Models;

namespace WebAPI.Helpers
{
    /// <summary>
    /// Data access helper for Container resources.
    /// </summary>
    public class ContainerSQLHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SomiodConnectionString"].ConnectionString;

        // GET /applications/{appId}/containers
        public static List<Container> GetContainers(int applicationId)
        {
            var list = new List<Container>();

            const string query =
                "SELECT Id, ResourceName, CreationDateTime, ApplicationId " +
                "FROM Containers WHERE ApplicationId = @appId";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@appId", applicationId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(LoadContainer(reader));
                }
            }

            return list;
        }

        // GET /applications/{appId}/containers/{name}
        public static Container GetContainer(int applicationId, string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return null;

            const string query =
                "SELECT Id, ResourceName, CreationDateTime, ApplicationId " +
                "FROM Containers WHERE ApplicationId = @appId AND ResourceName = @rn";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@appId", applicationId);
                cmd.Parameters.AddWithValue("@rn", resourceName);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? LoadContainer(reader) : null;
                }
            }
        }

        // POST /applications/{appId}/containers
        public static Container CreateContainer(Container container)
        {
            if (container == null)
                return null;

            if (string.IsNullOrWhiteSpace(container.ResourceName))
                return null;

            if (container.CreationDateTime == default)
                container.CreationDateTime = DateTime.UtcNow;

            const string insert =
                "INSERT INTO Containers (ResourceName, CreationDateTime, ApplicationId) " +
                "VALUES (@rn, @dt, @appId)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@rn", container.ResourceName);
                cmd.Parameters.AddWithValue("@dt", container.CreationDateTime);
                cmd.Parameters.AddWithValue("@appId", container.ApplicationId);

                conn.Open();

                return cmd.ExecuteNonQuery() > 0
                    ? GetContainer(container.ApplicationId, container.ResourceName)
                    : null;
            }
        }

        // DELETE /applications/{appId}/containers/{name}
        public static Container DeleteContainer(int applicationId, string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return null;

            var existing = GetContainer(applicationId, resourceName);
            if (existing == null)
                return null;

            const string delete =
                "DELETE FROM Containers WHERE ApplicationId = @appId AND ResourceName = @rn";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(delete, conn))
            {
                cmd.Parameters.AddWithValue("@appId", applicationId);
                cmd.Parameters.AddWithValue("@rn", resourceName);
                conn.Open();

                return cmd.ExecuteNonQuery() > 0 ? existing : null;
            }
        }

        // ---------- PRIVATE ----------
        private static Container LoadContainer(SqlDataReader reader)
        {
            return new Container
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ResourceName = reader.GetString(reader.GetOrdinal("ResourceName")),
                CreationDateTime = reader.GetDateTime(reader.GetOrdinal("CreationDateTime")),
                ApplicationId = reader.GetInt32(reader.GetOrdinal("ApplicationId"))
            };
        }
    }
}