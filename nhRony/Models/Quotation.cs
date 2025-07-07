using Microsoft.EntityFrameworkCore.Metadata;

namespace ClearingAndForwarding.Models
{
    public class Quotation
    {
        public int Id { get; set; }
        public int ExpenseHeadID { get; set; }
        public string? ItemName { get; set; }
        public decimal Amount { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public int? EmployeeId { get; set; }
        public string? Remarks { get; set; }
        public int? RestCont { get; set; }
        public decimal? OCFirstCont { get; set; }
        public decimal? OCRestCont { get;set; }
        public int? ItemCategoryId { get; set; }
  
        public ItemCategory? ItemCategories { get; set; }
        public ExpenseHead? ExpenseHead { get; set; }
        public Employee? Employee { get; set; }

    }
}
