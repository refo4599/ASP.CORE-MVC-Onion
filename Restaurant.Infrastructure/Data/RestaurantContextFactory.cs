using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Restaurant.Infrastructure.Data
{
    public class RestaurantContextFactory : IDesignTimeDbContextFactory<RestaurantContext>
    {
        public RestaurantContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RestaurantContext>();

            // ✅ هنا بتحدد الاتصال بقاعدة البيانات (لو مش متوفر appsettings)
            optionsBuilder.UseSqlServer("Server=.;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

            return new RestaurantContext(optionsBuilder.Options);
        }
    }
}
