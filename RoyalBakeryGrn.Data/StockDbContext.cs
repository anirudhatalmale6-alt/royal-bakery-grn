using Microsoft.EntityFrameworkCore;
using RoyalBakeryGrn.Data.Entities;

namespace RoyalBakeryGrn.Data
{
    public class StockDbContext : DbContext
    {
        // For runtime DI
        public StockDbContext(DbContextOptions<StockDbContext> options)
            : base(options) { }

        // Parameterless constructor for design-time migrations
        public StockDbContext() { }

        public DbSet<GRN> GRNs { get; set; }
        public DbSet<GRNItem> GRNItems { get; set; }

        public DbSet<GRNAdjustmentRequest> GRNAdjustmentRequest { get; set; }

        public DbSet<GRNAdjustmentRequestItem> GRNAdjustmentRequestItem { get; set; }

        public DbSet<Clearance> Clearances { get; set; }





        public DbSet<Stock> Stocks { get; set; }

        // Optional: if you want to access MenuItems from this context
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=.\\SQLEXPRESS;Database=RoyalBakery;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set decimal precision for GRNItem.Price
            modelBuilder.Entity<GRNItem>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Set decimal precision for Stock.Quantity
            modelBuilder.Entity<Stock>()
                .Property(s => s.Quantity)
                .HasColumnType("decimal(18,2)");

            // Optional: Map MENUITEM to existing MenuItems table
            // modelBuilder.Entity<MENUITEM>()
            //     .ToTable("MenuItems");
        }
    }
}
