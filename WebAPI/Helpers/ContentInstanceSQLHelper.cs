using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using WebAPI.Models;

namespace WebAPI.Helpers
{
    public class ContentInstanceSQLHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SomiodConnectionString"].ConnectionString;

        // GET /applications/{appId}/containers/{containerId}/contentInstances
        public static List<ContentInstance> GetContentInstances(int containerId)
        {
            var list = new List<ContentInstance>();

            const string query =
                "SELECT Id, ResourceName, ContentType, Content, CreationDateTime, ContainerId " +
                "FROM ContentInstances WHERE ContainerId = @cid";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@cid", containerId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(LoadContentInstance(reader));
                }
            }

            return list;
        }

        // GET /applications/{appId}/containers/{containerId}/contentInstances/{id}
        public static ContentInstance GetContentInstance(int id)
        {
            const string query =
                "SELECT Id, ResourceName, ContentType, Content, CreationDateTime, ContainerId " +
                "FROM ContentInstances WHERE Id = @id";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? LoadContentInstance(reader) : null;
                }
            }
        }

        // POST /applications/{appId}/containers/{containerId}/contentInstances
        public static ContentInstance CreateContentInstance(ContentInstance ci)
        {
            if (ci == null ||
                string.IsNullOrWhiteSpace(ci.ResourceName) ||
                string.IsNullOrWhiteSpace(ci.ContentType) ||
                string.IsNullOrWhiteSpace(ci.Content))
                return null;

            if (ci.CreationDateTime == default)
                ci.CreationDateTime = DateTime.UtcNow;

            const string insert =
                "INSERT INTO ContentInstances " +
                "(ResourceName, ContentType, Content, CreationDateTime, ContainerId) " +
                "VALUES (@rn, @ct, @content, @dt, @cid)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@rn", ci.ResourceName);
                cmd.Parameters.AddWithValue("@ct", ci.ContentType);
                cmd.Parameters.AddWithValue("@content", ci.Content);
                cmd.Parameters.AddWithValue("@dt", ci.CreationDateTime);
                cmd.Parameters.AddWithValue("@cid", ci.ContainerId);

                conn.Open();

                return cmd.ExecuteNonQuery() > 0
                    ? GetContentInstance(
                        Convert.ToInt32(
                            new SqlCommand("SELECT SCOPE_IDENTITY()", conn).ExecuteScalar()))
                    : null;
            }
        }

        // DELETE /applications/{appId}/containers/{containerId}/contentInstances/{id}
        public static ContentInstance DeleteContentInstance(int id)
        {
            var existing = GetContentInstance(id);
            if (existing == null)
                return null;

            const string delete =
                "DELETE FROM ContentInstances WHERE Id = @id";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(delete, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                return cmd.ExecuteNonQuery() > 0 ? existing : null;
            }
        }

        // ---------- PRIVATE ----------
        private static ContentInstance LoadContentInstance(SqlDataReader reader)
        {
            return new ContentInstance
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ResourceName = reader.GetString(reader.GetOrdinal("ResourceName")),
                ContentType = reader.GetString(reader.GetOrdinal("ContentType")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                CreationDateTime = reader.GetDateTime(reader.GetOrdinal("CreationDateTime")),
                ContainerId = reader.GetInt32(reader.GetOrdinal("ContainerId"))
            };
        }
    }
}
