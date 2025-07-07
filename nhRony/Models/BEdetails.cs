using System.ComponentModel.DataAnnotations.Schema;

namespace ClearingAndForwarding.Models
{
    public class BEdetails
    {
        public int Id { get; set; } // Primary key
        public string? FileNo { get; set; }
        public string BENo { get; set; }
        public DateOnly BEDate { get; set; }
        public string? LCNo { get; set; }
        public DateOnly? LCDate { get; set; }
        public string? ItemName { get; set; }
        public string? Quantity { get; set; }
        public string? UOM { get; set; } // Unit of Measurement
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public string? InvoiceNo { get; set; }
        public DateOnly? InvoiceDate { get; set; }
        public string? UnitCode { get; set; }
        public decimal? AssessableValue { get; set; }
        public decimal? Weight { get; set; }
        public string? Shipper { get; set; }
        public DateOnly? ArrivalDate { get; set; }
        public DateOnly? ODRecDate { get; set; }
        public DateOnly? AssessmentDate { get; set; }
        public DateOnly? DODate { get; set; } // Delivery Order Date
        public DateOnly? DeliveryDate { get; set; }
        public string? OnChassis { get; set; }
        public int? ContainerNo { get; set; }
        public string? ContainerSize { get; set; }
        public string? FileURL { get; set; }
        public string? Remarks { get; set; }
        public string? UserCode { get; set; }
        public int? ItemCategoryId { get; set; }
        public ItemCategory? ItemCategory { get; set; }
    }
    public class BEdetailsViewModel
    {
        public IEnumerable<BEdetails>? BEdetails { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string? SearchString { get; set; }
    }

}
