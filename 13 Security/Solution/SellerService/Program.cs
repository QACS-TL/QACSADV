
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SellerService.Infrastructure;
using SellerService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using System.Reflection.Metadata.Ecma335;

namespace SellerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<SellerContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("sqlestateagentdata")));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var sellerContext = scope.ServiceProvider.GetRequiredService<SellerContext>();
                sellerContext.Database.EnsureCreated();
                sellerContext.Seed();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/sellerstest",  () =>
               "Hello");

            app.MapGet("/sellers", async (SellerContext db) =>
                await db.Sellers.ToListAsync());

            app.MapGet("/sellers/{id}", async (int id, SellerContext db) =>
                await db.Sellers.FindAsync(id)
                    is Seller seller
                        ? Results.Ok(seller)
                        : Results.NotFound()).RequireAuthorization();

            app.MapGet("/sellersbyname/{surname, firstname}", async (string surname, string firstname, SellerContext db) =>
                await db.Sellers.FirstOrDefaultAsync(s => s.Surname == surname && s.FirstName == firstname)
                    is Seller seller
                        ? Results.Ok(seller)
                        : Results.NotFound()).RequireAuthorization(); ;

            app.MapPost("/sellers", async (Seller seller, SellerContext db) =>
            {
                db.Sellers.Add(seller);
                await db.SaveChangesAsync();

                return Results.Created($"/sellers/{seller.Id}", seller);
            }).RequireAuthorization(); ;

            app.MapPut("/sellers/{id}", async (int id, Seller inputSeller, SellerContext db) =>
            {
                var seller = await db.Sellers.FindAsync(id);

                if (seller is null) return Results.NotFound();

                seller.Surname = inputSeller.Surname;
                seller.FirstName = inputSeller.FirstName;
                seller.Address = inputSeller.Address;
                seller.Postcode = inputSeller.Postcode;
                seller.Phone = inputSeller.Phone;

                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization(); ;

            app.MapDelete("/sellers/{id}", async (int id, SellerContext db) =>
            {
                if (await db.Sellers.FindAsync(id) is Seller seller)
                {
                    db.Sellers.Remove(seller);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            }).RequireAuthorization(); ;

            app.MapPost("/login", (User loginUser) =>
            {
                var user = UserStore.Users.FirstOrDefault(u => u.UserName == loginUser.UserName && u.Password == loginUser.Password);

                if (user is null)
                {
                    return Results.Unauthorized();
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: builder.Configuration["Jwt:Issuer"],
                    audience: builder.Configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Results.Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            });


            app.Run();
        }
    }
}
