using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ClearingAndForwarding.Models 
{ 
    public class ExpensesViewModel
    {
        public List<Expenses> Expenses { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
