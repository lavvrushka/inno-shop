using UserManagement.Application.Common.Exeptions;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagement.Infrastructure.Services
{
    public class AuthTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenService(IConfiguration configuration, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role.Name)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString().Replace("-", ""),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["RefreshTokenExpirationDays"]))
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return refreshToken.Token;
        }

        public async Task RevokeRefreshTokenAsync()
        {
            var token = ExtractTokenFromHeader() ?? throw new UnauthorizedAccessException("Token is missing.");
            var refreshToken = _unitOfWork.RefreshTokens.GetByTokenAsync(token).Result;
            if (refreshToken != null)
            {
                await _unitOfWork.RefreshTokens.DeleteAsync(refreshToken);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<(string newAccessToken, string RefreshToken)> RefreshTokensAsync(string token)
        {

            var oldToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
            if (oldToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }
            if (oldToken.Expires < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token has expired.");
            }
            var user = await _unitOfWork.Users.GetByIdAsync(oldToken.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var newAccessToken = await GenerateAccessToken(user);

            return (newAccessToken, oldToken.Token);
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

        public async Task<User> AuthenticateUserAsync(string token)
        {
            var userId = ExtractUserIdFromToken(token)
                ?? throw new UnauthorizedAccessException("Invalid token.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            return user;
        }

    }
}
