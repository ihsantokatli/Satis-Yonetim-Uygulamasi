using FSD.Core.Entities;
using FSD.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FSD.WebUI.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            // Varsayılan: Son 30 gün
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddDays(-30);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            // Bitiş: günün sonu (23:59:59.999)
            endDate = endDate.Value.Date.AddDays(1).AddTicks(-1);

            // Satışları tarih aralığına göre filtrele
            var salesQuery = _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .Include(s => s.User)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate);

            var sales = await salesQuery.ToListAsync();

            // Genel toplam
            ViewBag.GeneralTotal = sales.Sum(s => s.TotalAmount);
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            // Ürün bazlı özet
            var productSummary = sales
                .SelectMany(s => s.SaleDetails)
                .GroupBy(sd => sd.Product.Name)
                .Select(g => new ProductReportItem
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            ViewBag.ProductSummary = productSummary;

            return View(sales);
        }
    }

    // Rapor için ViewModel
    public class ProductReportItem
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}