using Application.Abstraction;
using Domain.DbTables;
using Domain.User;
using Infrastructure.Abstraction;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Application.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly Hash _hash;

        public UserService(IUserRepository userRepository, IConfiguration config, Hash hash)
        {
            _userRepository = userRepository;
            _config = config;
            _hash = hash;
        }

        public async Task<string?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null) return null;

            if (!_hash.Verify(dto.Password, user.PasswordHash)) return null;

            return GenerateJwtToken(user);
        }

        public async Task<string?> RegisterAsync(UserRegisterDto dto)
        {
            var exists = await _userRepository.UserExistsByEmailAsync(dto.Email);
            if (exists) throw new Exception("User existent");

            var hashedPassword = _hash.Generate(dto.Password);
            var user = await _userRepository.CreateUserAsync(dto.Email, hashedPassword, dto.FullName);

            return GenerateJwtToken(user);
        }

        public async Task<UserReadDto?> GetUserInfoAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }
        public async Task<UserReadDto?> PatchUserDataAsync(int userId, UserUpdateDataDto dto)
        {
            return await _userRepository.PatchUserDataAsync(userId, dto);
        }

        public async Task<bool> ChangePasswordAsync(int userId, UserChangePasswordDto dto)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null) return false;

            if (!_hash.Verify(dto.CurrentPassword, user.PasswordHash))
                return false;

            var newHash = _hash.Generate(dto.NewPassword);
            await _userRepository.UpdatePasswordAsync(user, newHash);
            return true;
        }

        private string GenerateJwtToken(UserTable user)
        {
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("FullName", user.FullName)
            };

            var jwtKey = _config["Jwt:Key"] ?? string.Empty;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
