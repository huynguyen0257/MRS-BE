using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data
{
    public class MRSContext : IdentityDbContext<User>
    {
        public MRSContext() : base((new DbContextOptionsBuilder())
            .UseLazyLoadingProxies()
            //.UseSqlServer(@"Server=localhost;Database=MRSDB;user id=sa;password=1234;Trusted_Connection=True;Integrated Security=false;") //Huy
            //.UseSqlServer(@"Server=.;Database=MRSDB;user id=sa;password=1;Trusted_Connection=True;Integrated Security=false;") //Bình
            .UseSqlServer(@"Server=mlh2.database.windows.net;Database=MRSDB;user id=minh;password=tyz#25071998;Trusted_Connection=True;Integrated Security=false;") //Server
            .Options)
        {

        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DeliveryData> DeliveryData { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PopularProduct> PopularProducts { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Shop> Shop { get; set; }
        public DbSet<WareHouse> WareHouses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
            .HasOne<Category>(c => c.Category)
            .WithMany(u => u.Products)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<PopularProduct>()
            .HasOne<Product>(c => c.Product)
            .WithMany(u => u.PopularProducts)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PopularProduct>()
            .HasOne<User>(c => c.User)
            .WithMany(u => u.PopularProducts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<WareHouse>()
            .HasOne(c => c.Product)
            .WithOne(u => u.WareHouse)
            .HasForeignKey<WareHouse>(w => w.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cart>()
            .HasOne<WareHouse>(c => c.WareHouse)
            .WithMany(u => u.Carts)
            .HasForeignKey(w => w.WareHouseId);

            modelBuilder.Entity<Cart>()
            .HasOne<User>(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
            .HasOne<Order>(c => c.Order)
            .WithMany(o => o.Carts)
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
            .HasOne<Order>(c => c.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        }
        public void Commit()
        {
            base.SaveChanges();
        }
    }
}
