using Application.Abstraction;
using Domain.DbTables;
using Domain.User;
using Infrastructure.Abstraction;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace Application.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly Hash _hash;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IConfiguration config, Hash hash, IEmailService emailService)
        {
            _userRepository = userRepository;
            _config = config;
            _hash = hash;
            _emailService = emailService;
        }

        public async Task<string?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null) return null;

            if (!_hash.Verify(dto.Password, user.PasswordHash)) return null;

            if (!user.IsEmailVerified)
            {
                
                var resendToken = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
                var resendExpiry = DateTime.UtcNow.AddMinutes(15);
                await _userRepository.SetEmailVerificationTokenAsync(user, resendToken, resendExpiry);
                await _emailService.SendEmailVerificationAsync(user.Email, user.FullName, resendToken);
                throw new Exception("Email neverificat");
            }

            return GenerateJwtToken(user);
        }

        public async Task<bool> RegisterAsync(UserRegisterDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                if (existingUser.IsEmailVerified)
                    throw new Exception("User existent");

                // Utilizatorul există dar nu a verificat email-ul — retrimitem codul
                var newToken = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
                var newExpiry = DateTime.UtcNow.AddMinutes(15);
                await _userRepository.SetEmailVerificationTokenAsync(existingUser, newToken, newExpiry);
                await _emailService.SendEmailVerificationAsync(existingUser.Email, existingUser.FullName, newToken);
                return true;
            }

            var hashedPassword = _hash.Generate(dto.Password);
            var user = await _userRepository.CreateUserAsync(dto.Email, hashedPassword, dto.FullName);

            var resetToken = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15);
            await _userRepository.SetEmailVerificationTokenAsync(user, resetToken, expiry);
            await _emailService.SendEmailVerificationAsync(user.Email, user.FullName, resetToken);

            return true;
        }

        public async Task<string?> VerifyEmailAsync(VerifyEmailDto dto)
        {
            var user = await _userRepository.VerifyEmailAsync(dto.Email, dto.Token);
            if (user == null) return null;

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

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            var resetToken = (RandomNumberGenerator.GetInt32(100000, 1000000)).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(15);

            await _userRepository.SetPasswordResetTokenAsync(user, resetToken, expiry);

            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetToken);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var newHash = _hash.Generate(dto.NewPassword);
            return await _userRepository.ResetPasswordWithTokenAsync(dto.Email, dto.Token, newHash);
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
