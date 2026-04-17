using aspp.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace aspp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ===== CORE =====
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<RoomTransaction> RoomTransactions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceCondition> DeviceConditions { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Violation> Violations { get; set; }

        // ===== CONFIG RELATION =====
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Staff - Department
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Staffs)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Staff - Role
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Role)
                .WithMany(r => r.Staffs)
                .HasForeignKey(s => s.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}   