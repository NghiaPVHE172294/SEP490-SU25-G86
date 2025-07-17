namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService
{
    public interface ICacheService
    {
        Task SetAsync(string key, string value, TimeSpan ttl);
        Task<string> GetAsync(string key);
        Task RemoveAsync(string key);
    }
}
