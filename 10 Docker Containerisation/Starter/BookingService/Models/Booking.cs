using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    [Table("booking")]
    public class Booking
    {
        [Column("BOOKING_ID")]
        [Key]
        public int Id { get; set; }

        [Column("BUYER_ID")]
        public int BuyerId { get; set; }

        [Column("PROPERTY_ID")]
        public int PropertyId { get; set; }

        [Column("TIME")]
        public DateTime? Time { get; set; }
    }
}
