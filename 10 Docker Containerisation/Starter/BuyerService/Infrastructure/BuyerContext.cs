using BuyerService.Models;
using Microsoft.EntityFrameworkCore;

namespace BuyerService.Infrastructure
{
    public class BuyerContext : DbContext
    {
        public BuyerContext(DbContextOptions<BuyerContext> options) : base(options)
        {

        }

        public DbSet<Buyer> Buyers { get; set; }
    }
}

