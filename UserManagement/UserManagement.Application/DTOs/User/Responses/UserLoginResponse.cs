namespace UserManagement.Application.DTOs.User.Responses
{
    public record UserLoginResponse(
      string AccessToken,
      string RefreshToken

  );
}
