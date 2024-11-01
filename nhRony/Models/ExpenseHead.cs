namespace ClearingAndForwarding.Models
{
    public class ExpenseHead
    {
        public int Id { get; set; }
        public string ExpenseName { get; set; } = string.Empty;
        public string ExpenseType { get; set; } = string.Empty;

        // Navigation property for ExpenseDetails
        public List<ExpenseDetails> ExpenseDetails { get; set; } = new List<ExpenseDetails>();
    }
}
