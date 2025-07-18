using Microsoft.EntityFrameworkCore;
using TeleworkManagementAPI.Models;

namespace TeleworkManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<TeleworkRequest> TeleworkRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Manager)
                .WithMany()
                .HasForeignKey(c => c.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeleworkRequest>()
                .HasOne(tr => tr.Employee)
                .WithMany(e => e.TeleworkRequests)
                .HasForeignKey(tr => tr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeleworkRequest>()
                .HasOne(tr => tr.ProcessedByManager)
                .WithMany(e => e.ProcessedRequests)
                .HasForeignKey(tr => tr.ProcessedByManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuration des index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<TeleworkRequest>()
                .HasIndex(tr => tr.TeleworkDate);

            modelBuilder.Entity<TeleworkRequest>()
                .HasIndex(tr => tr.Status);
        }
    }
} 