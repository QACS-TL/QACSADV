using AutoFixture;
using SellerService.Models;

namespace SellerService.Infrastructure
{
    public static class SellerSeeder
    {
        public static void Seed(this SellerContext buyerContext)
        {
            if (!buyerContext.Sellers.Any())
            {
                Fixture fixture = new Fixture();
                fixture.Customize<Seller>(seller => seller.Without(p => p.Id));
                //--- The next two lines add 100 rows to your database
                List<Seller> sellers = fixture.CreateMany<Seller>(100).ToList();
                buyerContext.AddRange(sellers);
                buyerContext.SaveChanges();
            }
        }
    }
}

