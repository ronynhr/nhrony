namespace ClearingAndForwarding.Models
{
    public class DeliveryType
    {
        public int Id { get; set; }
        public string DeliveryTypeName { get; set; } = string.Empty;
        public int? UserCode { get; set; } = 0;
    }
    public class ItemGroup
    {
        public int Id { get; set; }
        public string ItemGroupName { get; set; } = string.Empty;
        public int? UserCode { get; set; } = 0;
    }
    public class ItemCategory
    {
        public int Id { get; set; }
        public required string CategoryName { get; set; }
        public string? Remarks { get; set; }
    }

}
