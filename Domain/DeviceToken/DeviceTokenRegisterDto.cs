namespace Domain.DeviceToken
{
    public class DeviceTokenRegisterDto
    {
        public string Token { get; set; } = null!;
        public string Platform { get; set; } = null!; // "android" or "ios"
    }
}
