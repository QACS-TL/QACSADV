using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SellerService.Models
{
    [Table("seller")]
    public class Seller
    {
        public Seller()
        {
            //Properties = null;
        }

        [Column("SELLER_ID")]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("FIRST_NAME")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        [Column("SURNAME")]
        public string Surname { get; set; }

        [Required]
        [StringLength(255)]
        [Column("ADDRESS")]
        public string Address { get; set; }

        [Required]
        [StringLength(255)]
        [Column("POSTCODE")]
        public string Postcode { get; set; }

        [Required]
        [StringLength(20)]
        [Column("PHONE")]
        public string Phone { get; set; }

        public object Clone()
        {
            return new Seller
            {
                Id = this.Id,
                FirstName = this.FirstName,
                Surname = this.Surname,
                Address = this.Address,
                Postcode = this.Postcode,
                Phone = this.Phone
            };
        }
        public bool Equals(Seller? other)
        {
            return Id == other.Id;
        }
    }
}
