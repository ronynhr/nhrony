using ClearingAndForwarding.Models;
using Microsoft.EntityFrameworkCore;

namespace ClearingAndForwarding.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) : base(contextOptions) { }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<ExpenseHead> ExpenseHead { get; set; }
        public DbSet<ExpenseDetails> ExpenseDetails { get; set; }
        public DbSet<Account> Accounts { get; set; }


    }
}