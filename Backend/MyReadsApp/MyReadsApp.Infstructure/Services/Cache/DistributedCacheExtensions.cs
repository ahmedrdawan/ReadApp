using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MyReadsApp.Infstructure.Services.Cache
{
    internal static class DistributedCacheExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string key)
        {
            string? json = await cache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        public static async Task SetRecordAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            TimeSpan? expiration = null)
        {
            string json = JsonSerializer.Serialize(value, JsonOptions);

            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            });
        }
    }
}
