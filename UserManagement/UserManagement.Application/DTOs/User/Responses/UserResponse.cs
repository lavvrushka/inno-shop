namespace UserManagement.Application.DTOs.User.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }

        public DateTime EmailVerifiedAt { get; set; }
    }
}
