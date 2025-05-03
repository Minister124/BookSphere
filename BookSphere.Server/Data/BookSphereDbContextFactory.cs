using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BookSphere.Data
{
    public class BookSphereDbContextFactory : IDesignTimeDbContextFactory<BookSphereDbContext>
    {
        public BookSphereDbContext CreateDbContext(string[] args = null)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BookSphereDbContext>();
            var connectionString = configuration.GetConnectionString("DB");
            optionsBuilder.UseNpgsql(connectionString);

            return new BookSphereDbContext(optionsBuilder.Options);
        }
    }
}