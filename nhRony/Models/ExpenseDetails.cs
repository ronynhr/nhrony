namespace ClearingAndForwarding.Models
{
    public class ExpenseDetails
    {
        public int Id { get; set; }

        // Foreign key to Expenses
        public int ExpenseId { get; set; }
        public Expenses? Expense { get; set; } 

        // Foreign key for related ExpenseHead
        public int ExpenseHeadId { get; set; }
        public ExpenseHead? ExpenseHead { get; set; } // Foreign key to ExpenseHead

        public decimal Amount { get; set; } // The amount for the specific head of account
    
    }
}
