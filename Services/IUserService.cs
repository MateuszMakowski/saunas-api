public interface IUserService
{
    Task<User> GetUserById(string id);
    Task<IEnumerable<User>> GetAllUsers();
    Task CreateUser(User user);
    Task UpdateUser(string id, User user);
    Task DeleteUser(string id);
}
