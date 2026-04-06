namespace Domain.User
{
    public class VerifyEmailDto
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
