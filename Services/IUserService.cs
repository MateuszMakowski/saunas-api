public interface IUserService
{
    Task<User> GetUserById(string id);
    Task<User> GetUserByEmail(string email);
    Task<bool> IsEmailUnique(string email);
    Task<IEnumerable<User>> GetAllUsers();
    Task CreateUser(User user);
    Task UpdateUser(string id, User user);
    Task DeleteUser(string id);
    User HashUserPassword(string password);
    Task<User> AuthenticateUser(string username, string password);
    bool VerifyPassword(string providedPassword, string storedHash, byte[] storedSalt);
    string GenerateJwtToken(User user);

}
