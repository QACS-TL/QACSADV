using Microsoft.EntityFrameworkCore;
using PropertyService.Infrastructure;
using PropertyService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; //Needed for Cors

// Add services to the container.
//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            //policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

builder.Services.AddDbContext<PropertyContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("sqlestateagentdata")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/properties", async (PropertyContext db) =>
    await db.Properties.ToListAsync());

app.MapGet("/properties/{id}", async (int id, PropertyContext db) =>
    await db.Properties.FindAsync(id)
        is Property property
            ? Results.Ok(property)
            : Results.NotFound());

app.MapPost("/Properties", async (Property property, PropertyContext db) =>
{
    db.Properties.Add(property);
    await db.SaveChangesAsync();

    return Results.Created($"/properties/{property.Id}", property);
});

app.MapPut("/Properties/{id}", async (int id, Property inputProperty, PropertyContext db) =>
{
    var property = await db.Properties.FindAsync(id);

    if (property is null) return Results.NotFound();

    property.Address = inputProperty.Address;
    property.Postcode = inputProperty.Postcode;
    property.Type = inputProperty.Type;
    property.Bedrooms = inputProperty.Bedrooms;
    property.Bathrooms = inputProperty.Bathrooms;
    property.Garden = inputProperty.Garden;
    property.Price = inputProperty.Price;
    property.Status = inputProperty.Status;
    property.SellerId = inputProperty.SellerId;
    property.BuyerId = inputProperty.BuyerId;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/Properties/{id}", async (int id, PropertyContext db) =>
{
    if (await db.Properties.FindAsync(id) is Property property)
    {
        db.Properties.Remove(property);
        await db.SaveChangesAsync();

        var http = new HttpClient();
        string url = $"https://localhost:7186/bookingsByPropertyId/{id}";
        HttpResponseMessage response = await http.DeleteAsync(url);

        return response.IsSuccessStatusCode ? Results.NoContent() : Results.NotFound(); // Results.NoContent();

    }

    return Results.NotFound();
});

app.Run();

