using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderHistoryWebApi.Models;

namespace OrderHistoryWebApi.Data
{
    public class OrderHistoryDbContext : DbContext
    {
        public OrderHistoryDbContext(DbContextOptions<OrderHistoryDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
                       
        }

        public DbSet<OrderHistoryWebApi.Models.OrderHistory> OrderHistory { get; set; }       

        public DbSet<OrderHistoryWebApi.Models.Product> Product { get; set; }

        public DbSet<OrderHistoryWebApi.Models.Store> Store { get; set; }
        
    }
}
