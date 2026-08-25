using FSD.Core.Entities;
using FSD.Data.Context;
using FSD.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSD.WebUI.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CurrencyService _currencyService;
        private const string CartSessionKey = "Cart";

        public SaleController(AppDbContext context)
        {
            _context = context;
            _currencyService = new CurrencyService();
        }

        // SATIŞ EKRANI (Ana Sayfa)
        public async Task<IActionResult> Index()
        {
            // Ürünleri getir (stokta olanlar)
            var products = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .Include(p => p.Category)
                .ToListAsync();

            // Güncel kurları çek
            var rates = await _currencyService.GetCurrentRatesAsync();

            ViewBag.Products = products;
            ViewBag.UsdRate = rates.ContainsKey("USD") ? rates["USD"] : 47.57m;
            ViewBag.EurRate = rates.ContainsKey("EUR") ? rates["EUR"] : 54.98m;

            // Sepeti getir
            var cart = GetCart();
            ViewBag.CartTotal = cart.Sum(c => c.Total);

            return View();
        }

        // SEPETE ÜRÜN EKLE (AJAX ile çağrılacak)
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, decimal? customPrice)
        {
            var product = _context.Products.Find(productId);
            if (product == null || product.StockQuantity < quantity)
                return Json(new { success = false, message = "Yetersiz stok veya ürün bulunamadı" });

            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    UnitPrice = customPrice ?? product.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);

            return Json(new
            {
                success = true,
                cartTotal = cart.Sum(c => c.Total),
                itemCount = cart.Count
            });
        }

        // SEPETTEN ÜRÜN ÇIKAR
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return Json(new { success = true, cartTotal = cart.Sum(c => c.Total) });
        }

        // SATIŞI TAMAMLA
        [HttpPost]
        public async Task<IActionResult> CompleteSale()
        {
            var cart = GetCart();
            if (!cart.Any())
                return Json(new { success = false, message = "Sepet boş" });

            // Kullanıcı ID'sini al
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr);

            var sale = new Sale
            {
                UserId = userId,
                TotalAmount = cart.Sum(c => c.Total),
                SaleDetails = cart.Select(c => new SaleDetail
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice
                }).ToList()
            };

            // Stok düşür ve stok hareketi kaydet
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                product.StockQuantity -= item.Quantity;

                _context.StockMovements.Add(new StockMovement
                {
                    ProductId = item.ProductId,
                    Type = MovementType.Out,
                    Quantity = item.Quantity
                });
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // Sepeti temizle
            HttpContext.Session.Remove(CartSessionKey);

            return Json(new { success = true, saleId = sale.Id });
        }

        // FİŞ SAYFASI
        public async Task<IActionResult> Receipt(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();
            return View(sale);
        }

        // ===== YARDIMCI METODLAR =====

        private List<CartItem> GetCart()
        {
            var session = HttpContext.Session.GetString(CartSessionKey);
            return session == null
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(session);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var cart = GetCart();
            return Json(cart);
        }
    }


    // SEPET İÇİN YARDIMCI CLASS
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }

}