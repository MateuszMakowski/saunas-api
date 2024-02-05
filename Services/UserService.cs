using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class UserService : IUserService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<User> GetUserById(string id)
    {
        return await _userRepository.GetUserById(id);
    }


    public async Task<User> GetUserByEmail(string email)
    {
        return await _userRepository.GetUserByEmail(email);
    }

    public async Task<bool> IsEmailUnique(string email)
    {
        return await _userRepository.IsEmailUnique(email);

    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _userRepository.GetAllUsers();
    }

    public async Task CreateUser(User user)
    {
        User hashedUser = HashUserPassword(user.Password);
        user.Password = hashedUser.Password;
        user.Salt = hashedUser.Salt;
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

    public User HashUserPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        var user = new User();
        string saltBase64 = Convert.ToBase64String(salt);
        // Hashowanie hasła z użyciem soli
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        user.Password = hashed;
        user.Salt = saltBase64;
        return user;
        // Zapisz użytkownika wraz z zahashowanym hasłem i solą do bazy danych
    }

    // Ta metoda powinna być zaimplementowana, aby uwierzytelnić użytkownika z bazą danych
    public async Task<User> AuthenticateUser(string email, string password)
    {
        // Pobierz użytkownika z bazy danych
        var user = await GetUserByEmail(email);

        if (user != null)
        {
            // Przykład, gdzie 'user.Password' to zahashowane hasło, a 'user.Salt' to sól z bazy danych
            var verified = VerifyPassword(password, user.Password, Convert.FromBase64String(user.Salt));
            if (verified)
            {
                return user; // Uwierzytelnianie powiodło się
            }
        }

        return null; // Uwierzytelnianie nie powiodło się
    }


    public bool VerifyPassword(string providedPassword, string storedHash, byte[] storedSalt)
    {
        // Hashowanie podanego hasła z użyciem soli zapisanej w bazie danych
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: providedPassword,
            salt: storedSalt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // Porównanie zahashowanego hasła z hasłem zapisanym w bazie danych
        return hashed == storedHash;
    }


    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                // Dodaj więcej claimów według potrzeb
            };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
