
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult Secret()
        {
            return Ok("Tajne dane dostępne tylko dla uwierzytelnionych użytkowników");
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            bool uniqueEmail = await _userService.IsEmailUnique(createUserDto.Email);
            if (!uniqueEmail)
            {
                return BadRequest("Email już istnieje.");
            }
            else if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = createUserDto.Password,
                Role = createUserDto.Role
                // Mapowanie pozostałych pól
            };

            await _userService.CreateUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userService.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            await _userService.UpdateUser(id, user);
            return NoContent();
        }

        // DELETE: api/Users/{id}
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            await _userService.DeleteUser(id);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // Przykład logiki uwierzytelniania (powinieneś zaimplementować rzeczywiste sprawdzanie w bazie danych)
            var user = await _userService.AuthenticateUser(loginModel.Email, loginModel.Password);

            if (user != null)
            {
                // Jeśli użytkownik jest prawidłowo uwierzytelniony, generujemy dla niego token JWT
                var token = _userService.GenerateJwtToken(user); // Teraz przekazujemy obiekt User do metody generującej token
                return Ok(new { Token = token });
            }

            // Jeśli dane uwierzytelniające są nieprawidłowe, zwracamy Unauthorized
            return Unauthorized();
        }
    }

}

// "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI2NWJmNjlhOGYxMTk3MzE3OTYwYmQwNGMiLCJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJyb2xlIjoidXNlciIsIm5iZiI6MTcwNzA0MzI1OSwiZXhwIjoxNzA3MDUwNDU5LCJpYXQiOjE3MDcwNDMyNTksImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VRDsgGDLwhDHnbd-bN-pHVpx9cBtTz0b3Yex5enmdgc"