using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TechMoves.API;
using TechMoves.API.Data;
using TechMoves.API.Models;

namespace TechMoves.Tests.IntergrationTests
{
    public class CustomWebFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated();

                if (!db.Clients.Any())
                {
                    db.Clients.Add(new Client
                    {
                        Id = 1,
                        Name = "Test Client",
                        ContactDetails = "test@mail.com",
                        Region = "GP"
                    });

                    db.SaveChanges();
                }
            });
        }
    }
}