using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PropertyService.Infrastructure;
using PropertyService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PropertyContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("sqlestateagentdata")));

builder.Services.AddCap(options =>
{
    options.UseEntityFramework<PropertyContext>();
    options.UseSqlServer(
            builder.Configuration.GetConnectionString("sqlestateagentdata"));

    options.UseDashboard(d =>
    {
        d.AllowAnonymousExplicit = true;
    });
   
    options.UseRabbitMQ(options =>
    {
        options.ConnectionFactoryOptions = options =>
        {
            options.Ssl.Enabled = false;
            options.HostName = "rabbitmq";
            options.UserName = "guest";
            options.Password = "guest";
            options.Port = 5672;
        };
    });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var propertyContext = scope.ServiceProvider.GetRequiredService<PropertyContext>();
    propertyContext.Database.EnsureCreated();
    propertyContext.Seed();
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

app.MapDelete("/Properties/{id}", async (int id, PropertyContext db, ICapPublisher capPublisher) =>
{
    if (await db.Properties.FindAsync(id) is Property property)
    {
        db.Properties.Remove(property);
        await db.SaveChangesAsync();

        PropertyDeletedEvent propertyDeletedEvent =
            new PropertyDeletedEvent { PropertyId = property.Id };
        var content = JsonConvert.SerializeObject(propertyDeletedEvent);
        await capPublisher.PublishAsync("PropertyDeleted", content);
        return Results.NoContent();


    }

    return Results.NotFound();
});

app.Run();

