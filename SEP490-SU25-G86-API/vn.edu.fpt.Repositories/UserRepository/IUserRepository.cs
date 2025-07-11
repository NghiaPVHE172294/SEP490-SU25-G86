﻿using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByAccountIdAsync(int accountId);
        Task UpdateUserAsync(User user);
    }
}
