using Domain.User;

namespace Application.Abstraction
{
    public interface IUserService
    {
        Task<string?> LoginAsync(UserLoginDto dto);
        Task<string?> RegisterAsync(UserRegisterDto dto);
        Task<UserReadDto?> GetUserInfoAsync(int userId);
        Task<UserReadDto?> PatchUserDataAsync(int userId, UserUpdateDataDto dto);
        Task<bool> ChangePasswordAsync(int userId, UserChangePasswordDto dto);
    }
}