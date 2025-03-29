using System.Text;
using BookSphere.Data;
using BookSphere.Hubs;
using BookSphere.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BookSphereDbContext>(options =>
        options.UseNpgsql(
                builder.Configuration.GetConnectionString("DB"),
                NpgSqlOptions => NpgSqlOptions.MigrationsAssembly("BookSphere")
        )
);

builder.Services.AddAuthentication(options => 
{
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
        options.TokenValidationParameters = new TokenValidationParameters
        {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                )
        };

        options.Events = new JwtBearerEvents
        {
                OnMessageReceived = context =>
                {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/booksphere"))
                        {
                                context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                }
        };
});

builder.Services.AddAuthorization(options =>
{
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireStaffRole", policy => policy.RequireRole("Staff, Admin"));
        options.AddPolicy("RequireMemberRole", policy => policy.RequireRole("Member"));
});

builder.Services.AddCors(options =>
{
        options.AddPolicy("AllowReactApp", policy => 
        {
                policy.WithOrigins(builder.Configuration["AllowedOrigins"]!.Split(';'))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials(); //Need this for signalR
                });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>(); //for handling errors and exceptions

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowReactApp");

app.MapHub<BookHubs>("/bookshpere"); //sends to everyone of domain that has /booksphere

app.Run();
