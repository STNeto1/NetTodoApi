using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTodoApi.DTO.Auth;
using NetTodoApi.Models;

namespace NetTodoApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult> PostRegister(RegisterDto payload)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DatabaseContext.Users'  is null.");
            }

            var existingUser = await _context.Users.Where(q => q.Email == payload.Email).SingleOrDefaultAsync();
            if (existingUser is not null)
            {
                return Problem("Email already in use", null, 400);
            }

            //var existingUser = _context.Users.FindAsync({})
            var pwdHash = BCrypt.Net.BCrypt.HashPassword(payload.Password);

            var newUser = new User
            {
                Email = payload.Email,
                Password = pwdHash
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<JwtResponse>> PostLogin(LoginDto payload)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DatabaseContext.Users'  is null.");
            }

            var existingUser = await _context.Users.SingleAsync(q => q.Email == payload.Email);
            if (existingUser is null)
            {
                return Problem("Invalid credentials", null, 400);
            }

            if (!BCrypt.Net.BCrypt.Verify(payload.Password, existingUser.Password))
            {
                return Problem("Invalid credentials", null, 400);
            }


            return Ok(CreateToken(existingUser));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ProfileResponse>> GetProfile()
        {
            var claimsPrincipal = this.User;

            var sub = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (sub is null)
            {
                return Problem("Unauthorized", null, 400);
            }

            var user = await _context.Users.Where(u => u.Id.ToString() == sub).FirstOrDefaultAsync();

            return Ok(new ProfileResponse
            {
                Id = user.Id,
                Email = user.Email
            });
        }

        private JwtResponse CreateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var userClaims = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = userClaims,
                Expires = DateTime.Now.AddDays(1),
                Issuer = "MyDotnetAPI",
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenJwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenJwt);

            return new JwtResponse
            {
                Token = token
            };
        }
    }
}