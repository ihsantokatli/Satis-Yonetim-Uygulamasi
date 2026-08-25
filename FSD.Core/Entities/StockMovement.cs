using System;

namespace FSD.Core.Entities
{
    public class StockMovement : BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.Now;
        public MovementType Type { get; set; }
        public int Quantity { get; set; }
    }
}