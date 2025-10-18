using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System;

namespace contract_monthly_claim_system_cs.Extensions
{
    /// <summary>
    /// Extension methods for session management
    /// Provides strongly-typed session storage methods without recursion
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
        /// Sets an integer value in the session using native session methods
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <param name="value">The integer value to store</param>
        public static void SetInt32(this ISession session, string key, int value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Gets an integer value from the session using native session methods
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <returns>The integer value or null</returns>
        public static int? GetInt32(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null || data.Length < 4)
            {
                return null;
            }
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Sets a string value in the session using native session methods
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <param name="value">The string value to store</param>
        public static void SetString(this ISession session, string key, string value)
        {
            session.SetString(key, value);
        }

        /// <summary>
        /// Gets a string value from the session using native session methods
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <returns>The string value or null</returns>
        public static string GetString(this ISession session, string key)
        {
            return session.GetString(key);
        }
    }
}