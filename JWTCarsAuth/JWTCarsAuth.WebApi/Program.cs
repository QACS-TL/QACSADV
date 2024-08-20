using JWTCarsAuth.WebApi.Interface;
using JWTCarsAuth.WebApi.Models;
using JWTCarsAuth.WebApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; //Needed for Cors
// Add services to the container.

//builder.Services.AddScoped<IEmployees, EmployeeRepository>();

//builder.Services.AddDbContext<DatabaseContext>
//    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnection")));
builder.Services.AddTransient<ICarRepository, CarRepository>();
builder.Services.AddControllers();

// Cors allows client requests to be made from same (localhost) machine
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            //policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
            policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
        });
});

//Donot forgot to add ConnectionStrings as "dbConnection" to the appsetting.json file
var connectionString = builder.Configuration.GetConnectionString("dbConnection") ?? throw new InvalidOperationException("Connection string 'Carsdb_jwt' not found.");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
