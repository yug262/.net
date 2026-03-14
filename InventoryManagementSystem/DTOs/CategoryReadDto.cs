namespace InventoryAPI.DTOs
{
    public class CategoryReadDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
