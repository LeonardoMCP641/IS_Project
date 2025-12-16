using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using WebAPI.Models;

namespace WebAPI.Helpers
{
    /// <summary>
    /// Data access helper for Subscription resources.
    /// </summary>
    public class SubscriptionSQLHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["SomiodConnectionString"].ConnectionString;

        // GET /applications/{appId}/containers/{containerId}/subscriptions
        public static List<Subscription> GetSubscriptions(int containerId)
        {
            var list = new List<Subscription>();

            const string query =
                "SELECT Id, Endpoint, CreationDateTime, ContainerId " +
                "FROM Subscriptions WHERE ContainerId = @cid";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@cid", containerId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(LoadSubscription(reader));
                }
            }

            return list;
        }

        // GET /applications/{appId}/containers/{containerId}/subscriptions/{id}
        public static Subscription GetSubscription(int id)
        {
            const string query =
                "SELECT Id, Endpoint, CreationDateTime, ContainerId " +
                "FROM Subscriptions WHERE Id = @id";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? LoadSubscription(reader) : null;
                }
            }
        }

        // POST /applications/{appId}/containers/{containerId}/subscriptions
        public static Subscription CreateSubscription(Subscription sub)
        {
            if (sub == null)
                return null;

            if (string.IsNullOrWhiteSpace(sub.Endpoint))
                return null;

            if (sub.CreationDateTime == default)
                sub.CreationDateTime = DateTime.UtcNow;

            const string insert =
                "INSERT INTO Subscriptions (Endpoint, CreationDateTime, ContainerId) " +
                "VALUES (@ep, @dt, @cid)";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@ep", sub.Endpoint);
                cmd.Parameters.AddWithValue("@dt", sub.CreationDateTime);
                cmd.Parameters.AddWithValue("@cid", sub.ContainerId);

                conn.Open();

                return cmd.ExecuteNonQuery() > 0 ? sub : null;
            }
        }

        // DELETE /applications/{appId}/containers/{containerId}/subscriptions/{id}
        public static Subscription DeleteSubscription(int id)
        {
            var existing = GetSubscription(id);
            if (existing == null)
                return null;

            const string delete =
                "DELETE FROM Subscriptions WHERE Id = @id";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(delete, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                return cmd.ExecuteNonQuery() > 0 ? existing : null;
            }
        }

        // ---------- PRIVATE ----------
        private static Subscription LoadSubscription(SqlDataReader reader)
        {
            return new Subscription
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Endpoint = reader.GetString(reader.GetOrdinal("Endpoint")),
                CreationDateTime = reader.GetDateTime(reader.GetOrdinal("CreationDateTime")),
                ContainerId = reader.GetInt32(reader.GetOrdinal("ContainerId"))
            };
        }
    }
}
