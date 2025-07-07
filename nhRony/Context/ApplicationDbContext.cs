using ClearingAndForwarding.Controllers;
using ClearingAndForwarding.Models;
using Microsoft.EntityFrameworkCore;

namespace ClearingAndForwarding.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) : base(contextOptions) { }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<ExpenseHead> ExpenseHead { get; set; }
        public DbSet<ExpenseDetails> ExpenseDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UnitName> UnitName { get; set; } 
        public DbSet<BEdetails> BEdetails { get; set; }
        public DbSet<BillTypes> BillTypes { get; set; }
        public DbSet<Quotation> Quotation { get; set; } = default!;
        public DbSet<Requisition> Requisition { get; set; } = default!;
        public DbSet<RequisitionDetails> RequisitionDetails { get; set; } = default!;
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<DeliveryType> DeliveryTypes { get; set; }
        public DbSet<ItemGroup> ItemGroup { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Requisition>()
                .HasMany(r => r.RequisitionDetails)
                .WithOne(d => d.Requisition)
                .HasForeignKey(d => d.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expenses>()
                .HasMany(r => r.ExpenseDetails)
                .WithOne(d => d.Expense)
                .HasForeignKey(d => d.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}