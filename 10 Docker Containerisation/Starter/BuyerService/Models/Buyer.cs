using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuyerService.Models
{
    [Table("buyer")]
    public class Buyer
    {
        [Column("BUYER_ID")]
        [Key]
        public int Id { get; set; }
        [Column("FIRST_NAME")]
        public string? FirstName { get; set; }
        [Column("SURNAME")]
        public string? Surname { get; set; }
        [Column("ADDRESS")]
        public string? Address { get; set; }
        [Column("POSTCODE")]
        public string? Postcode { get; set; }
        [Column("PHONE")]
        public string? Phone { get; set; }
    }
}
