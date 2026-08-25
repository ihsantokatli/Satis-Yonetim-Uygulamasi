using FSD.Core.Entities;
using FSD.Data.Context;
using FSD.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FSD.WebUI.Controllers
{
    [Authorize] // Login olmayan giremez
    public class ProductController : Controller
    {
        private readonly Repository<Product> _productRepo;
        private readonly Repository<Category> _categoryRepo;
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
            _productRepo = new Repository<Product>(context);
            _categoryRepo = new Repository<Category>(context);
        }

        // Ürün listesi - Herkes görebilir
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

        // Ürün ekleme sayfası - Sadece Admin
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepo.AddAsync(product);
                await _productRepo.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name");
            return View(product);
        }

        // Ürün düzenleme - Sadece Admin
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepo.Update(product);
                await _productRepo.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _categoryRepo.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }
    }
}