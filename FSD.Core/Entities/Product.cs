namespace FSD.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<SaleDetail>? SaleDetails { get; set; }
        public ICollection<StockMovement>? StockMovements { get; set; }
    }
}