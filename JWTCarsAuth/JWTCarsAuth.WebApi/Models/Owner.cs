using System.ComponentModel.DataAnnotations.Schema;

namespace JWTCarsAuth.WebApi.Models
{
    [Table("owner", Schema = "cardb_jwt")]
    public class Owner
    {
        [Column(name: "ownerid")]
        public int OwnerId { get; set; }
        [Column(name: "firstname")]
        public string? FirstName { get; set; }
        [Column(name: "lastname")]
        public string? LastName { get; set; }

    }
}
