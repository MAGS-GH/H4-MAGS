using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly AppDBContext _context;

        public UserController(IConfiguration configuration, AppDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.CreatedAt = DateTime.UtcNow.AddHours(2);
            user.UpdatedAt = DateTime.UtcNow.AddHours(2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok($"New user created: {user.Username}");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (userFromDb == null)
            {
                return BadRequest("User not found.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, userFromDb.PasswordHash);

            if (!isPasswordValid)
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(userFromDb);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Retrieve the JWT settings from configuration
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Add issuer and audience to the JwtSecurityToken
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
