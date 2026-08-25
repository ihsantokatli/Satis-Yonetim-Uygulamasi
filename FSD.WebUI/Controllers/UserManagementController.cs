using FSD.Core.Entities;
using FSD.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FSD.WebUI.Controllers
{
    [Authorize(Roles = "CompanyAdmin")] // Sadece Admin erişebilir
    public class UserManagementController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserManagementController(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // KULLANICI LİSTESİ
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // YENİ KULLANICI EKLEME SAYFASI
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                UserType = model.UserType,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Role ata
                string roleName = model.UserType == UserType.CompanyAdmin ? "CompanyAdmin" : "CompanyUser";
                await _userManager.AddToRoleAsync(user, roleName);

                return RedirectToAction(nameof(Index));
            }

            // Hata varsa ekle
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }


            return View(model);
        }
        // KULLANICI SİL
        /*public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            // Kendi kendini silmesin
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
        */
        // KULLANICI PASİF/AKTİF YAP
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            // Kendi kendini pasif yapamasın
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "Kendi hesabınızın durumunu değiştiremezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive; // Aktifse pasif, pasifse aktif yap
            await _userManager.UpdateAsync(user);

            string status = user.IsActive ? "aktif" : "pasif";
            TempData["Success"] = $"{user.UserName} kullanıcısı {status} durumuna getirildi.";

            return RedirectToAction(nameof(Index));
        }
    }
}