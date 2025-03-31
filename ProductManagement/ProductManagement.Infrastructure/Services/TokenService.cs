using Microsoft.AspNetCore.Http;
using ProductManagement.Domain.Interfaces.IServices;
using System.IdentityModel.Tokens.Jwt;

namespace ProductManagement.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService( IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? ExtractTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }

            if (authorizationHeader.StartsWith("Bearer "))
            {
                return authorizationHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }

        public Guid? ExtractUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    throw new UnauthorizedAccessException("Invalid token format.");
                }

                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid");
                if (userIdClaim == null)
                {
                    throw new UnauthorizedAccessException("UserId not found in token.");
                }

                return Guid.Parse(userIdClaim.Value);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Token validation failed.", ex);
            }
        }
    }
}
