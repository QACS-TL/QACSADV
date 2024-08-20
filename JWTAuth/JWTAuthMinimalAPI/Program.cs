using JWTAuthMinimalAPI.WebApi.Interface;
using JWTAuthMinimalAPI.WebApi.Models;
using JWTAuthMinimalAPI.WebApi.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("dbConnection") ?? throw new InvalidOperationException("Connection string 'Forum.Users' not found.");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IEmployees, EmployeeRepository>();

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("/Employee", async (DatabaseContext dbContext) => 
{
    IEmployees _employee = new EmployeeRepository(dbContext);
    return await Task.FromResult(_employee.GetEmployeeDetails());
});

app.MapPost("/Post", async (IConfiguration config, DatabaseContext dbContext, UserInfo _userData) =>
{
    if (_userData != null && _userData.Email != null && _userData.Password != null)
    {
        var user = await GetUser(dbContext, _userData.Email, _userData.Password);

        if (user != null)
        {
            //create claims details based on the user information
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("DisplayName", user.DisplayName),
                        new Claim("UserName", user.UserName),
                        new Claim("Email", user.Email)
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);
            var result = Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
            return await Task.FromResult(result);
        }
        else
        {
            return BadRequest("Invalid credentials");
        }
    }
    else
    {
        return BadRequest();
    }
});

async Task<UserInfo> GetUser(DatabaseContext dbContext, string email, string password)
{
    return await dbContext.UserInfos.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
}

app.Run();
