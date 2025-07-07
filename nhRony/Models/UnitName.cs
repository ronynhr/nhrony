using Microsoft.EntityFrameworkCore.Metadata;

namespace ClearingAndForwarding.Models
{
    public class UnitName
    {
        public int Id { get; set; }
        public required string CodeName { get; set; } 
        public required string CompanyName { get; set; }
        public string? BINno { get; set; }
        public string? TINno { get; set; }
        public string? Address { get; set; }
        public string? Type { get; set; }

    }
}
