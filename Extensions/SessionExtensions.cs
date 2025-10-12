using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace contract_monthly_claim_system_cs.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }

        public static void SetInt32(this ISession session, string key, int value)
        {
            session.Set(key, value);
        }

        public static int? GetInt32(this ISession session, string key)
        {
            return session.Get<int?>(key);
        }

        public static void SetString(this ISession session, string key, string value)
        {
            session.Set(key, value);
        }

        public static string GetString(this ISession session, string key)
        {
            return session.Get<string>(key);
        }
    }
}