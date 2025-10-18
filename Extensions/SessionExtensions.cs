using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System;

namespace contract_monthly_claim_system_cs.Extensions
{
    /// <summary>
    /// Custom extension methods for session management
    /// Provides strongly-typed session storage methods without conflicting with built-in extensions
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Sets a complex object in the session using JSON serialization
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <param name="value">The value to store</param>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        /// <summary>
        /// Gets a complex object from the session using JSON deserialization
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <returns>The deserialized value or default</returns>
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// Safely gets an integer from session with custom method name to avoid conflicts
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <returns>The integer value or null</returns>
        public static int? GetSessionInt(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null || data.Length == 0)
            {
                return null;
            }
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Safely sets an integer in session with custom method name to avoid conflicts
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <param name="value">The integer value to store</param>
        public static void SetSessionInt(this ISession session, string key, int value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Safely gets a string from session with custom method name to avoid conflicts
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <returns>The string value or empty string if not found</returns>
        public static string GetSessionString(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }
            return System.Text.Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Safely sets a string in session with custom method name to avoid conflicts
        /// </summary>
        /// <param name="session">The session instance</param>
        /// <param name="key">The session key</param>
        /// <param name="value">The string value to store</param>
        public static void SetSessionString(this ISession session, string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                session.Remove(key);
            }
            else
            {
                session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
            }
        }
    }
}