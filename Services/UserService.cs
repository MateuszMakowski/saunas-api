public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetUserById(string id)
    {
        return await _userRepository.GetUserById(id);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _userRepository.GetAllUsers();
    }

    public async Task CreateUser(User user)
    {
        await _userRepository.CreateUser(user);
    }

    public async Task UpdateUser(string id, User user)
    {
        await _userRepository.UpdateUser(id, user);
    }

    public async Task DeleteUser(string id)
    {
        await _userRepository.DeleteUser(id);
    }
}
