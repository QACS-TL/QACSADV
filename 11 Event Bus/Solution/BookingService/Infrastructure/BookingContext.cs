using Microsoft.EntityFrameworkCore;
using BookingService.Models;

namespace BookingService.Infrastructure
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        {

        }

        public DbSet<Booking> Bookings { get; set; }
    }
}
