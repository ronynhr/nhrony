
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearingAndForwarding.Models
{
    public class Expenses
    {
        public int Id { get; set; }
        public int BEno { get; set; }
        public string? FileNo { get; set; }
        public string BillType { get; set; } = string.Empty;
        public DateOnly BillDate { get; set; }
        public string? BillNo { get; set; }
        public DateTime PostingDate { get; set; }
        public int EmployeeId { get; set; }
        public decimal? Amount { get; set; }
        public int? BEdetailsID { get; set; }
        public int? RequisitionID { get; set; }

        public virtual Employee? Employee { get; set; }
        public List<ExpenseDetails> ExpenseDetails { get; set; } = new List<ExpenseDetails>();
        public BEdetails? Bedetails { get; set; }
        public Requisition? Requisition { get; set; }
    }
    public class ExpensesViewModel
    {
        public List<Expenses>? Expenses { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchString { get; set; }
    }
}