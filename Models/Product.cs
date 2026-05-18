using System;

namespace InventoryApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal SellingPrice { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; } // Alert threshold
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Calculated field for dashboard/reporting
        public decimal TotalValue => SellingPrice * StockQuantity;
    }
}
