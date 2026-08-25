using System;
using System.Collections.Generic;

namespace FSD.Core.Entities
{
    public class Sale : BaseEntity
    {
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public AppUser? User { get; set; }
        public ICollection<SaleDetail>? SaleDetails { get; set; }
    }
}