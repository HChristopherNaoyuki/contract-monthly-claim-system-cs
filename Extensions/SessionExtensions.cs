using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System;

namespace contract_monthly_claim_system_cs.Extensions
{
    /// <summary>
    /// Extension methods for session management
    /// Provides strongly-typed session storage methods
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Sets a value in the session using JSON serialization
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <param name="value">The value to store</param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        /// <summary>
        /// Gets a value from the session using JSON deserialization
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <returns>The deserialized value or default</returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// Extension method to get integer from session
        /// </summary>
        public static int? GetInt32(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Extension method to set integer in session
        /// </summary>
        public static void SetInt32(this ISession session, string key, int value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Extension method to get string from session
        /// </summary>
        public static string GetString(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return System.Text.Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Extension method to set string in session
        /// </summary>
        public static void SetString(this ISession session, string key, string value)
        {
            session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Gets session data as byte array
        /// </summary>
        private static byte[] Get(this ISession session, string key)
        {
            byte[] value = null;
            session.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Sets session data as byte array
        /// </summary>
        private static void Set(this ISession session, string key, byte[] value)
        {
            session.Set(key, value);
        }
    }
}