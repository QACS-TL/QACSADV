using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PropertyService.Models
{
    [Table("property")]
    public class Property
    {
        [Column("PROPERTY_ID")]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("ADDRESS")]
        public string Address { get; set; }

        [Required]
        [StringLength(255)]
        [Column("POSTCODE")]
        public string Postcode { get; set; }

        [Required]
        [StringLength(9)]
        [Column("TYPE")]
        public string Type { get; set; }

        [Column("NUMBER_OF_BEDROOMS")]
        public int Bedrooms { get; set; }

        [Column("NUMBER_OF_BATHROOMS")]
        public int Bathrooms { get; set; }

        [Required]
        [Column("GARDEN")]
        public bool Garden { get; set; }

        [Column("PRICE")]
        public decimal? Price { get; set; }

        [Required]
        [StringLength(9)]
        [Column("STATUS")]
        public string Status { get; set; }

        [Required]
        [Column("Seller_ID")]
        public int SellerId { get; set; }

        [Column("BUYER_ID")]
        public int? BuyerId { get; set; }
    }
}
