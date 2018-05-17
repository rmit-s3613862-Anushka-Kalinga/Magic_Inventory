using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Magic_Inventory.Models;

namespace Magic_Inventory.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<StoreInventory>().HasKey(x => new { x.StoreID, x.ProductID });
        }

        public DbSet<Magic_Inventory.Models.OrderHistory> OrderHistory { get; set; }

        public DbSet<Magic_Inventory.Models.Cart> Cart { get; set; }

        public DbSet<Magic_Inventory.Models.Product> Product { get; set; }

        public DbSet<Magic_Inventory.Models.Store> Store { get; set; }

        public DbSet<Magic_Inventory.Models.OwnerInventory> OwnerInventory { get; set; }

        public DbSet<Magic_Inventory.Models.StockRequest> StockRequest { get; set; }

        public DbSet<Magic_Inventory.Models.Cart> OrderDetail { get; set; }

        public DbSet<Magic_Inventory.Models.OrderHistory> CustomerOrder { get; set; }

        public DbSet<Magic_Inventory.Models.StoreInventory> StoreInventory { get; set; }
    }
}
