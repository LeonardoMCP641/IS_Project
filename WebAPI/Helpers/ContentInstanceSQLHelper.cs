using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using WebAPI.Models;

namespace WebAPI.Helpers
{
    /// <summary>
    /// Data access helper for ContentInstance resources.
    /// </summary>
    public class ContentInstanceSQLHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SomiodConnectionString"].ConnectionString;

        // GET /applications/{appId}/containers/{containerId}/contentInstances
        public static List<ContentInstance> GetContentInstances(int containerId)
        {
            var list = new List<ContentInstance>();

            const string query =
                "SELECT Id, Content, CreationDateTime, ContainerId " +
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
                "SELECT Id, Content, CreationDateTime, ContainerId " +
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
            if (ci == null)
                return null;

            if (string.IsNullOrWhiteSpace(ci.Content))
                return null;

            if (ci.CreationDateTime == default)
                ci.CreationDateTime = DateTime.UtcNow;

            const string insert =
                "INSERT INTO ContentInstances (Content, CreationDateTime, ContainerId) " +
                "VALUES (@content, @dt, @cid)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@content", ci.Content);
                cmd.Parameters.AddWithValue("@dt", ci.CreationDateTime);
                cmd.Parameters.AddWithValue("@cid", ci.ContainerId);

                conn.Open();

                return cmd.ExecuteNonQuery() > 0 ? ci : null;
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
                Content = reader.GetString(reader.GetOrdinal("Content")),
                CreationDateTime = reader.GetDateTime(reader.GetOrdinal("CreationDateTime")),
                ContainerId = reader.GetInt32(reader.GetOrdinal("ContainerId"))
            };
        }
    }
}