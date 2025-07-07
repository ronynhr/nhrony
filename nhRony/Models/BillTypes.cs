using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClearingAndForwarding.Models
{
    public class BillTypes
    {
        public int Id { get; set; }
        public required string BillTypeName { get; set; }
        public string? BillTypeDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
