using Domain.DbTables;
using Domain.User;

namespace Infrastructure.Abstraction
{
    public interface IUserRepository
    {
        Task<UserTable?> GetUserByEmailAsync(string email);
        Task<bool> UserExistsByEmailAsync(string email);
        Task<UserTable> CreateUserAsync(string email, string passwordHash, string fullName);
        Task<UserReadDto?> GetUserByIdAsync(int userId);
        Task<UserReadDto?> PatchUserDataAsync(int userId, UserUpdateDataDto dto);
        Task<UserTable?> GetUserEntityByIdAsync(int userId);
        Task UpdatePasswordAsync(UserTable user, string newPasswordHash);
        Task SetPasswordResetTokenAsync(UserTable user, string token, DateTime expiry);
        Task<bool> ResetPasswordWithTokenAsync(string email, string token, string newPasswordHash);
        Task SetEmailVerificationTokenAsync(UserTable user, string token, DateTime expiry);
        Task<UserTable?> VerifyEmailAsync(string email, string token);
    }
}
