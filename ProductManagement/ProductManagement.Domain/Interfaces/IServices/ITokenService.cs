namespace ProductManagement.Domain.Interfaces.IServices
{
    public interface ITokenService
    {
        Guid? ExtractUserIdFromToken(string token);
        string? ExtractTokenFromHeader();
    }
}
