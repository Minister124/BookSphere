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
        public DbSet<WishList> WishLists {get; set;}
        public DbSet<WishListItem> WishListsItems {get; set;}
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
    }
}
