using Domain.DbTables;
using Domain.User;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserTable?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<UserTable> CreateUserAsync(string email, string passwordHash, string fullName)
        {
            var user = new UserTable
            {
                Email = email,
                PasswordHash = passwordHash,
                FullName = fullName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserReadDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;
            return new UserReadDto
            {
                Email = user.Email,
                FullName = user.FullName
            };
        }
        public async Task<UserReadDto?> PatchUserDataAsync(int userId, UserUpdateDataDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            await _context.SaveChangesAsync();

            return new UserReadDto
            {
                Email = user.Email,
                FullName = user.FullName
            };
        }

        public async Task<UserTable?> GetUserEntityByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task UpdatePasswordAsync(UserTable user, string newPasswordHash)
        {
            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
        }
    }
}
