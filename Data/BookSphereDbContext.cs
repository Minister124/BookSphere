using BookSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSphere.Data;

public class BookSphereDbContext : DbContext
{
        public BookSphereDbContext(DbContextOptions<BookSphereDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Book> Books {get; set;}
        public DbSet<User> Users {get; set;}
        public DbSet<Order> Orders {get; set;}
        public DbSet<OrderItem> OrderItems {get; set;}
        public DbSet<Cart> Carts {get; set;}
        public DbSet<CartItem> CartItems {get; set;}
        public DbSet<Review> Reviews {get; set;}
        public DbSet<WhiteList> WhiteLists {get; set;}
        public DbSet<WhiteListItem> WhiteListsItems {get; set;}
        public DbSet<Announcement> Announcements {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
                .HasIndex(u => u.EmailAddress)
                .IsUnique();
                
        modelBuilder.Entity<User>()
            .HasIndex(u => u.MembershipId)
            .IsUnique()
            .HasFilter("\"MembershipId\" is not null");
            
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();
            
        modelBuilder.Entity<Book>()
            .HasMany(b => b.OrderItems)
            .WithOne(oi => oi.Book)
            .HasForeignKey(oi => oi.BookId);
            
        modelBuilder.Entity<User>() //one user has one whitelist
            .HasOne(u => u.WhiteList)
            .WithOne(w => w.User)
            .HasForeignKey<WhiteList>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        //one user has one cart
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        //user can have many order
        modelBuilder.Entity<User>()
            .HasMany(U => U.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        //order has many orderitems
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        //A user can have multiple reviews
        modelBuilder.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        //A book can have multiple reviews
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Reviews)
            .WithOne(r => r.Book)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        //one user can only review a book once
        modelBuilder.Entity<Review>()
            .HasIndex(r => new {r.UserId, r.BookId})
            .IsUnique();
    }
}
