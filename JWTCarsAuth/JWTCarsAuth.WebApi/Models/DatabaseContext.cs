using JWTCarsAuth.WebApi.Interface;
using Microsoft.EntityFrameworkCore;

namespace JWTCarsAuth.WebApi.Models
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car>? Cars { get; set; }
        public virtual DbSet<Owner>? Owners { get; set; }
        public virtual DbSet<User>? Users { get; set; }


    }
}
