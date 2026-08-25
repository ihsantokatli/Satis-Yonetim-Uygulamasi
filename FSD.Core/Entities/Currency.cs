using System;

namespace FSD.Core.Entities
{
    public class Currency : BaseEntity
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}