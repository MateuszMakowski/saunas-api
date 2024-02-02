using MongoDB.Bson;
using MongoDB.Driver;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDBSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _users = database.GetCollection<User>("Users");
    }

    public async Task<User> GetUserById(string id)
    {
        return await _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _users.Find(user => true).ToListAsync();
    }

    public async Task CreateUser(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateUser(string id, User userIn)
    {
        await _users.ReplaceOneAsync(user => user.Id == id, userIn);
    }

    public async Task DeleteUser(string id)
    {
        await _users.DeleteOneAsync(user => user.Id == id);
    }
}
