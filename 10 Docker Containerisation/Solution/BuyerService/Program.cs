
using BuyerService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BuyerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; //Needed for Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                            policy =>
                            {
                                //policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                            });
            });
            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<BuyerContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("sqlestateagentdata")));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var buyerContext = scope.ServiceProvider.GetRequiredService<BuyerContext>();
                    buyerContext.Database.EnsureCreated();
                    buyerContext.Seed();
                }
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
