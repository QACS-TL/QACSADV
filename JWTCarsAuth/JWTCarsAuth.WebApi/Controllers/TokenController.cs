using JWTCarsAuth.WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuth.WebApi.Controllers
{
    [Route("api/token")]
    [ApiController]
    //[EnableCors(""_myAllowSpecificOrigins"")]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly DatabaseContext _context;

        public TokenController(IConfiguration config, DatabaseContext context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(User _userData)
        {
            if (_userData != null && _userData.UserName != null && _userData.Password != null)
            {
                var user = await GetUser(_userData.UserName, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("Role", user.Role),
                        new Claim("UserName", user.UserName)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

                    var resp = new AuthorizationResponse
                    {
                        UserId = user.Id.ToString(),
                        AuthorizationToken = encodedJwt,
                        RefreshToken = string.Empty
                    };

                    return Ok(resp);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<User> GetUser(string UserName, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == UserName && u.Password == password);
        }

        public class AuthorizationResponse {
            public string UserId { get; set; }
            public string AuthorizationToken { get; set; }
            public string RefreshToken { get; set; }
        }

    }
}
