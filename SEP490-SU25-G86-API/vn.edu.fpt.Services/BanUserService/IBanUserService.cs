namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.BanUserService
{
    public interface IBanUserService
    {
        Task<bool> BanUserAsync(int userId);
        Task<bool> UnbanUserAsync(int userId);
    }
}
