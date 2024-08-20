using AutoFixture;
using BookingService.Models;

namespace BookingService.Infrastructure
{
    public static class BuyerSeeder
    {
        public static void Seed(this BookingContext bookingContext)
        {
            if (!bookingContext.Bookings.Any())
            {
                Fixture fixture = new Fixture();
                fixture.Customize<Booking>(booking => booking.Without(p => p.Id));
                //--- The next two lines add 100 rows to your database
                List<Booking> bookings = fixture.CreateMany<Booking>(100).ToList();
                bookingContext.AddRange(bookings);
                bookingContext.SaveChanges();
            }
        }
    }
}
