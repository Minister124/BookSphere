using BookSphere.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BookSphereDbContext>(options =>
        options.UseNpgsql(
                builder.Configuration.GetConnectionString("DB"),
                NpgSqlOptions => NpgSqlOptions.MigrationsAssembly("BookSphere")
        )
);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
