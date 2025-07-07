using Microsoft.EntityFrameworkCore.Metadata;

namespace ClearingAndForwarding.Models
{
    public class RequisitionDetails
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int ExpenseHeadId { get; set; }
        public decimal Amount { get; set; }

        public ExpenseHead? ExpenseHead { get; set; }
        public Requisition? Requisition { get; set; }
    }
}