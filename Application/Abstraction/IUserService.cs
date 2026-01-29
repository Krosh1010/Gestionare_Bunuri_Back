using Domain.User;

namespace Application.Abstraction
{
    public interface IUserService
    {
        Task<string?> LoginAsync(UserLoginDto dto);
        Task<string?> RegisterAsync(UserRegisterDto dto);
    }
}