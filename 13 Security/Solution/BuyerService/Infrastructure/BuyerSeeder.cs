using AutoFixture;
using BuyerService.Models;

namespace BuyerService.Infrastructure
{
    public static class BuyerSeeder
    {
        public static void Seed(this BuyerContext buyerContext)
        {
            if (!buyerContext.Buyers.Any())
            {
                Fixture fixture = new Fixture();
                fixture.Customize<Buyer>(buyer => buyer.Without(p => p.Id));
                //--- The next two lines add 100 rows to your database
                List<Buyer> buyers = fixture.CreateMany<Buyer>(100).ToList();
                buyerContext.AddRange(buyers);
                buyerContext.SaveChanges();
            }
        }
    }
}

