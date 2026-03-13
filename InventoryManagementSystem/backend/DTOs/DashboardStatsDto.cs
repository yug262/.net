namespace InventoryAPI.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal InventoryValue { get; set; }
        public int TotalSold { get; set; }
        public decimal TodayProfit { get; set; }
    }
}
