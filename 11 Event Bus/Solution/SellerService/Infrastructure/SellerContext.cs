using Microsoft.EntityFrameworkCore;
using SellerService.Models;

namespace SellerService.Infrastructure
{
    public class SellerContext : DbContext
    {
        public SellerContext(DbContextOptions<SellerContext> options) : base(options)
        {

        }

        public DbSet<Seller> Sellers { get; set; }
    }
}
