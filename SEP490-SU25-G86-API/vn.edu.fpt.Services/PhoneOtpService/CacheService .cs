using Microsoft.Extensions.Caching.Memory;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task SetAsync(string key, string value, TimeSpan ttl)
        {
            _cache.Set(key, value, ttl);
            return Task.CompletedTask;
        }

        public Task<string> GetAsync(string key)
        {
            _cache.TryGetValue(key, out string value);
            return Task.FromResult(value);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
