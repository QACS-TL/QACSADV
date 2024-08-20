using System.ComponentModel.DataAnnotations.Schema;

namespace JWTCarsAuth.WebApi.Models
{
    [Table("car", Schema = "cardb_jwt")]
    public class Car
    {
        [Column(name: "id")]
        public int ID { get; set; }
        [Column(name: "brand")]
        public string? Brand { get; set; }
        [Column(name: "colour")]
        public string? Colour { get; set; }
        [Column(name: "model")]
        public string? Model { get; set; }
        [Column(name: "price")]
        public int? Price { get; set; }
        [Column(name: "registration_number")]
        public string? Registration { get; set; }
        [Column(name: "year")]
        public int? Year { get; set; }
        [Column(name: "owner")]
        public int? Owner { get; set; }
    }
}
