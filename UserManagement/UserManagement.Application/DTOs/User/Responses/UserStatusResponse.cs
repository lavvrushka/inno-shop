namespace UserManagement.Application.DTOs.User.Responses
{
    public class UserStatusResponse
    {
        public bool IsActive { get; set; }

        public UserStatusResponse(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
