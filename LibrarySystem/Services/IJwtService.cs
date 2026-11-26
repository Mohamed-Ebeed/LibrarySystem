using LibrarySystem.Domain;

namespace LibrarySystem.Services
{
    public interface IJwtService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user);
    }
}