using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DbTables;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Asset
{
    public class DeviceTokenRepository : IDeviceTokenRepository
    {
        private readonly AppDbContext _context;

        public DeviceTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegisterTokenAsync(int userId, string token, string platform)
        {
            // Verificăm dacă tokenul există deja
            var existing = await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.Token == token);

            if (existing != null)
            {
                // Actualizăm userId și platform dacă tokenul există deja (posibil alt user s-a logat pe același dispozitiv)
                existing.UserId = userId;
                existing.Platform = platform;
                existing.LastUsedAt = DateTime.UtcNow;
            }
            else
            {
                _context.DeviceTokens.Add(new DeviceTokenTable
                {
                    UserId = userId,
                    Token = token,
                    Platform = platform,
                    CreatedAt = DateTime.UtcNow,
                    LastUsedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveTokenAsync(int userId, string token)
        {
            var deviceToken = await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.UserId == userId && dt.Token == token);

            if (deviceToken != null)
            {
                _context.DeviceTokens.Remove(deviceToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<DeviceTokenTable>> GetTokensByUserIdAsync(int userId)
        {
            return await _context.DeviceTokens
                .Where(dt => dt.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<int>> GetAllUserIdsWithTokensAsync()
        {
            return await _context.DeviceTokens
                .Select(dt => dt.UserId)
                .Distinct()
                .ToListAsync();
        }

        public async Task RemoveInvalidTokenAsync(string token)
        {
            var deviceToken = await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.Token == token);

            if (deviceToken != null)
            {
                _context.DeviceTokens.Remove(deviceToken);
                await _context.SaveChangesAsync();
            }
        }
    }
}
