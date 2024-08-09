using AutoFixture;
using PropertyService.Models;

namespace PropertyService.Infrastructure
{
    public static class PropertySeeder
    {
        public static void Seed(this PropertyContext propertyContext)
        {
            if (!propertyContext.Properties.Any())
            {
                Fixture fixture = new Fixture();
                fixture.Customize<Property>(seller => seller.Without(p => p.Id));
                //--- The next two lines add 100 rows to your database
                List<Property> properties = fixture.CreateMany<Property>(100).ToList();
                propertyContext.AddRange(properties);
                propertyContext.SaveChanges();
            }

        }
    }
}
