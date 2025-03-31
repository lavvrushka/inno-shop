namespace UserManagement.Application.DTOs.User.Responses
{
   public record UserRegisterResponse(
   string AccessToken,
   string RefreshToken);
}
