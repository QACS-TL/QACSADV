using Microsoft.EntityFrameworkCore;
using BookingService.Infrastructure;
using BookingService.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddControllers();

builder.Services.AddDbContext<BookingContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("sqlestateagentdata")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var bookingContext = scope.ServiceProvider.GetRequiredService<BookingContext>();
        bookingContext.Database.EnsureCreated();
        bookingContext.Seed();
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/bookings", async (BookingContext db) =>
    await db.Bookings.ToListAsync());

app.MapGet("/bookings/{id}", async (int id, BookingContext db) =>
    await db.Bookings.FindAsync(id)
        is Booking booking
            ? Results.Ok(booking)
            : Results.NotFound());

app.MapPost("/bookings", async (Booking booking, BookingContext db) =>
{
    var http = new HttpClient();

    //Check to see if PropertyId is valid
    string url = $"https://localhost:3012/properties/{booking.PropertyId}";
    HttpResponseMessage response = await http.GetAsync(url);
    string responseJson = response.Content.ReadAsStringAsync().Result;
    dynamic responseData = JsonConvert.DeserializeObject(responseJson);
    if (responseData == null || responseData["id"] != booking.PropertyId)
        return Results.NotFound();

    //Check to see if BuyerId is valid
    url = $"https://localhost:3011/api/buyer/buyers/{booking.BuyerId}";
    response = await http.GetAsync(url);
    responseJson = response.Content.ReadAsStringAsync().Result;
    responseData = JsonConvert.DeserializeObject(responseJson);
    if (responseData == null || responseData["id"] != booking.BuyerId)
        return Results.NotFound();

    db.Bookings.Add(booking);
    await db.SaveChangesAsync();

    return Results.Created($"/bookings/{booking.Id}", booking);
});

app.MapPut("/bookings/{id}", async (int id, Booking inputBooking, BookingContext db) =>
{
    var booking = await db.Bookings.FindAsync(id);

    if (booking is null) return Results.NotFound();

    booking.BuyerId = inputBooking.BuyerId;
    booking.PropertyId = inputBooking.PropertyId;
    booking.Time = inputBooking.Time;


    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/bookings/{id}", async (int id, BookingContext db) =>
{
    if (await db.Bookings.FindAsync(id) is Booking booking)
    {
        db.Bookings.Remove(booking);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.MapDelete("/bookingsByPropertyId/{id}", async (int id, BookingContext db) =>
{
    List<Booking> bookings = await db.Bookings.Where(b => b.PropertyId == id).ToListAsync();
    if (bookings == null || bookings.Count == 0)
    {
        return Results.NotFound();
    }
    foreach (Booking booking in bookings)
    {
        db.Bookings.Remove(booking);
    }
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

