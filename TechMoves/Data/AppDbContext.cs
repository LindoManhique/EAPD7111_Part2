using Microsoft.EntityFrameworkCore;
using TechMoves.Models;

namespace TechMoves.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client → Contract (1 to many)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.ClientId);

            // Contract → ServiceRequest (1 to many)
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(s => s.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(s => s.ContractId);

           
            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.Cost)
                .HasPrecision(18, 2);
        }
    }
}