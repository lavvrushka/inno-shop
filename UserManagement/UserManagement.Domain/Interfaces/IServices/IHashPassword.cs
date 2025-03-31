namespace UserManagement.Domain.Interfaces.IServices
{
    public interface IHashPassword
    {
        string Hash(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
