using UserManagement.Domain.Models;

namespace UserManagement.Domain.Interfaces.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshTokenAsync(Guid userId);
        Task RevokeRefreshTokenAsync();
        Task<(string newAccessToken, string RefreshToken)> RefreshTokensAsync(string token);
        Guid? ExtractUserIdFromToken(string token);
        string? ExtractTokenFromHeader();

        Task<User> AuthenticateUserAsync(string token);
    }
}
