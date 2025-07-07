using Microsoft.EntityFrameworkCore.Metadata;
using System.Runtime.CompilerServices;

namespace ClearingAndForwarding.Models
{
    public class Requisition
    {
        public int Id { get; set; }
        public int ExpenseID { get; set; }
        public int BEdetailsID { get; set; }
        public int EmployeeId { get; set; }
        public string? RequisitionNo { get; set; }
        public DateOnly RequisitionDate { get; set; }
        public decimal? RequisitionAmount { get; set; }
        public decimal? AdjustedAmount { get; set; }
        public bool Status { get; set; }

        public virtual Employee? Employee { get; set; }
        public BEdetails? Bedetails { get; set; }
        public List<RequisitionDetails> RequisitionDetails { get; set; } = [];
        public Expenses? Expenses { get; set; }
    }
    public class RequisitionViewModel
    {
        public List<Requisition>? Requisitions { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? SearchString { get; set; }
    }
}