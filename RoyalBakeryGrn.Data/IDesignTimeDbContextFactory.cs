using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RoyalBakeryGrn.Data
{
    public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
    {
        public StockDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-BEH394T\\SQLEXPRESS;Database=RoyalBakery;Trusted_Connection=True;TrustServerCertificate=True;");

            return new StockDbContext(optionsBuilder.Options);
        }
    }
}
