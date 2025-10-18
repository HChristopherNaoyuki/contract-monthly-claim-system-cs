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

        // Note: We don't override SetInt32, GetInt32, SetString, GetString
        // to avoid recursion with the built-in session methods
    }
}