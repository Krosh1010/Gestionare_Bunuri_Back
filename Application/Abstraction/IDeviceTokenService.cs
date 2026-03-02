namespace Application.Abstraction
{
    public interface IDeviceTokenService
    {
        Task RegisterTokenAsync(int userId, string token, string platform);
        Task RemoveTokenAsync(int userId, string token);
    }
}
