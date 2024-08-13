
using Microsoft.EntityFrameworkCore;
using SellerService.Infrastructure;
using SellerService.Models;
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

            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var sellerContext = scope.ServiceProvider.GetRequiredService<SellerContext>();
                    sellerContext.Database.EnsureCreated();
                    sellerContext.Seed();
                }
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/sellerstest",  () =>
               "Hello");

            app.MapGet("/sellers", async (SellerContext db) =>
                await db.Sellers.ToListAsync());

            //app.MapGet("/sellers/{id}", async (int id, SellerContext db) =>
            //    await db.Sellers.FindAsync(id)
            //        is Seller seller
            //            ? Results.Ok(seller)
            //            : Results.NotFound());

            //app.MapGet("/sellers/{surname, firstname}", async (string surname, string firstname, SellerContext db) =>
            //    await db.Sellers.FirstOrDefaultAsync(s => s.Surname == surname && s.FirstName == firstname)
            //        is Seller seller
            //            ? Results.Ok(seller)
            //            : Results.NotFound());

            //app.MapPost("/sellers", async (Seller seller, SellerContext db) =>
            //{
            //    db.Sellers.Add(seller);
            //    await db.SaveChangesAsync();

            //    return Results.Created($"/sellers/{seller.Id}", seller);
            //});

            //app.MapPut("/sellers/{id}", async (int id, Seller inputSeller, SellerContext db) =>
            //{
            //    var seller = await db.Sellers.FindAsync(id);

            //    if (seller is null) return Results.NotFound();

            //    seller.Surname = inputSeller.Surname;
            //    seller.FirstName = inputSeller.FirstName;
            //    seller.Address = inputSeller.Address;
            //    seller.Postcode = inputSeller.Postcode;
            //    seller.Phone = inputSeller.Phone;

            //    await db.SaveChangesAsync();

            //    return Results.NoContent();
            //});

            //app.MapDelete("/sellers/{id}", async (int id, SellerContext db) =>
            //{
            //    if (await db.Sellers.FindAsync(id) is Seller seller)
            //    {
            //        db.Sellers.Remove(seller);
            //        await db.SaveChangesAsync();
            //        return Results.NoContent();
            //    }

            //    return Results.NotFound();
            //});

            app.Run();
        }
    }
}
