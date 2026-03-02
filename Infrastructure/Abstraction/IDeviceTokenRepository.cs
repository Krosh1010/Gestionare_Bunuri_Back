using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DbTables;

namespace Infrastructure.Abstraction
{
    public interface IDeviceTokenRepository
    {
        Task RegisterTokenAsync(int userId, string token, string platform);
        Task RemoveTokenAsync(int userId, string token);
        Task<List<DeviceTokenTable>> GetTokensByUserIdAsync(int userId);
        Task<List<int>> GetAllUserIdsWithTokensAsync();
        Task RemoveInvalidTokenAsync(string token);
    }
}
