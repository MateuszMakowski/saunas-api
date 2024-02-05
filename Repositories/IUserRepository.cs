public interface IUserRepository
{
    Task<User> GetUserById(string id);
    Task<User> GetUserByEmail(string email);
    Task<IEnumerable<User>> GetAllUsers();
    Task CreateUser(User user);
    Task UpdateUser(string id, User user);
    Task DeleteUser(string id);
    Task<bool> IsEmailUnique(string email);
}