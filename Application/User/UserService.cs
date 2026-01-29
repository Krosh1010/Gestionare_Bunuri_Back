using Application.Abstraction;
using Domain.DbTables;
using Domain.User;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Application.User
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly Hash _hash;

        public UserService(AppDbContext context, IConfiguration config, Hash hash)
        {
            _context = context;
            _config = config;
            _hash = hash;
        }

        public async Task<string?> LoginAsync(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return null;

            if (!_hash.Verify(dto.Password, user.PasswordHash)) return null;

            return GenerateJwtToken(user);
        }

        public async Task<string?> RegisterAsync(UserRegisterDto dto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists) throw new Exception("User existent");

            var user = new UserTable
            {
                Email = dto.Email,
                PasswordHash = _hash.Generate(dto.Password),
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserTable user)
        {
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("FullName", user.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
