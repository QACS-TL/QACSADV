using BookingService.Infrastructure;
using DotNetCore.CAP;
using Events;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BookingService.DomainEventHandlers
{
    public class PropertyDeletedEventSubscriber
    {
        private readonly BookingContext _bookingContext;

        public PropertyDeletedEventSubscriber(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        [CapSubscribe("PropertyDeleted")]
        public async Task<IResult> Consumer(string content)
        {
            var property = JsonConvert.DeserializeObject<PropertyDeletedEvent>(content);

            var bookings = _bookingContext.Bookings.Where(b => b.PropertyId == property.PropertyId).ToList();
            if (bookings is null || bookings.Count() == 0)
                return Results.NotFound();
            _bookingContext.Bookings.RemoveRange(bookings);
            _bookingContext.SaveChanges();
            return Results.NoContent();

        }

    }
}
