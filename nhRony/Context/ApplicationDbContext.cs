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
            // Requisition → RequisitionDetails (One-to-Many)
            modelBuilder.Entity<RequisitionDetails>()
                .HasOne(rd => rd.Requisition)
                .WithMany(r => r.RequisitionDetails)
                .HasForeignKey(rd => rd.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade); // Or Restrict if you prefer
        
            // Requisition → Expenses (One-to-One or Optional-One)
            modelBuilder.Entity<Expenses>()
                .HasOne(e => e.Requisition)
                .WithOne(r => r.Expenses)
                .HasForeignKey<Expenses>(e => e.RequisitionID)
                .OnDelete(DeleteBehavior.Restrict); // 👈 important to avoid cascade conflicts
        
            // ExpenseDetails → Expense (Many-to-One) if needed
            modelBuilder.Entity<ExpenseDetails>()
                .HasOne(ed => ed.Expense)
                .WithMany(e => e.ExpenseDetails)
                .HasForeignKey(ed => ed.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        
            // RequisitionDetails → ExpenseHead (Many-to-One)
            modelBuilder.Entity<RequisitionDetails>()
                .HasOne(rd => rd.ExpenseHead)
                .WithMany()
                .HasForeignKey(rd => rd.ExpenseHeadId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
