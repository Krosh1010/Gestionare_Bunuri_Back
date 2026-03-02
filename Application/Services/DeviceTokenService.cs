using Application.Abstraction;
using Infrastructure.Abstraction;

namespace Application.Services
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;

        public DeviceTokenService(IDeviceTokenRepository deviceTokenRepository)
        {
            _deviceTokenRepository = deviceTokenRepository;
        }

        public async Task RegisterTokenAsync(int userId, string token, string platform)
        {
            await _deviceTokenRepository.RegisterTokenAsync(userId, token, platform);
        }

        public async Task RemoveTokenAsync(int userId, string token)
        {
            await _deviceTokenRepository.RemoveTokenAsync(userId, token);
        }
    }
}
