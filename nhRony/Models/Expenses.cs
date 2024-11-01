
namespace ClearingAndForwarding.Models
{
    public class Expenses
    {
        public int Id { get; set; }
        public int? BEno { get; set; }
        public string? FileNo { get; set; }
        public string BillType { get; set; } = string.Empty;
        public decimal BillAmount { get; set; }
        public DateOnly BillDate { get; set; }
        public string? BillNo { get; set; }
        public DateTime PostingDate { get; set; }
        public int EmployeeId { get; set; }


        public virtual Employee Employee { get; set; }
        public List<ExpenseDetails> ExpenseDetails { get; set; } = new List<ExpenseDetails>();

    }
}