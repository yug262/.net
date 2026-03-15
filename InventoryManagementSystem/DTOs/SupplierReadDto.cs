namespace InventoryAPI.DTOs
{
    public class SupplierReadDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
