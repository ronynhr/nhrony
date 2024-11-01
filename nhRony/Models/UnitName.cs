using Microsoft.EntityFrameworkCore.Metadata;

namespace ClearingAndForwarding.Models
{
    public class UnitName
    {
        public int Id { get; set; }
        public string CodeName { get; set; } 
        public string FullName { get; set; }

    }
}
