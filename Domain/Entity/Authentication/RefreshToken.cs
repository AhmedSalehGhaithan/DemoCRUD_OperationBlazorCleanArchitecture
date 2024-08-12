
namespace Domain.Entity.Authentication
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
    }
}
