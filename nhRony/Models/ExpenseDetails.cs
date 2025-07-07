
namespace ClearingAndForwarding.Models
{
    public class ExpenseDetails
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int ExpenseId { get; set; }
        public int ExpenseHeadId { get; set; }
        public int? QuotationID { get; set; }

        public Expenses? Expense { get; set; }
        public ExpenseHead? ExpenseHead { get; set; } 
        public Quotation? Quotation { get; set; }

    }
}

