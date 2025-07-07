using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClearingAndForwarding.Models
{
    public class Duties // Replace TableName with the actual name of your table/class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
        public int BEID { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Specify precision and scale for decimals
        public decimal CustomDuty { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Specify precision and scale for decimals
        public decimal PortCharge { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Specify precision and scale for decimals
        public decimal PortDemurrage { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Specify precision and scale for decimals
        public decimal SADemurrage { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Specify precision and scale for decimals
        public decimal DepotDemurrage { get; set; }



    }
}
