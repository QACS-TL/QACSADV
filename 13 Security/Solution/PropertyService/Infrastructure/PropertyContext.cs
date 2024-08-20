using Microsoft.EntityFrameworkCore;
using PropertyService.Models;

namespace PropertyService.Infrastructure
{
    public class PropertyContext : DbContext
    {
        public PropertyContext(DbContextOptions<PropertyContext> options) : base(options)
        {

        }

        public DbSet<Property> Properties { get; set; }
    }
}
