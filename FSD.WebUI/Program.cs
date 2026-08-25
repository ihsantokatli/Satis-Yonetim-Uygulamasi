using FSD.Core.Entities;
using FSD.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity 
builder.Services.AddIdentity<AppUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Session 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();        // Session middleware'i
app.UseAuthentication(); // Kimlik doðrulama
app.UseAuthorization();  // Yetkilendirme

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    // Veritabanýný güncelle (migration'larý otomatik uygula)
    context.Database.Migrate();

    // Rolleri oluþtur (yoksa)
    if (!await roleManager.RoleExistsAsync("CompanyAdmin"))
        await roleManager.CreateAsync(new IdentityRole<int>("CompanyAdmin"));

    if (!await roleManager.RoleExistsAsync("CompanyUser"))
        await roleManager.CreateAsync(new IdentityRole<int>("CompanyUser"));

    // Admin kullanýcýsý oluþtur (yoksa)
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = "admin",
            Email = "admin@fsd.com",
            FullName = "Firma Admin",
            UserType = UserType.CompanyAdmin,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "CompanyAdmin");
    }
    // Varsayýlan kategorileri ekle (yoksa)
    if (!context.Categories.Any())
    {
        var categories = new List<Category>
        {
            new Category { Name = "Elektronik" },
            new Category { Name = "Gýda" },
            new Category { Name = "Giyim" },
            new Category { Name = "Mobilya" },
            new Category { Name = "Kýrtasiye" }
        };
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }
}

app.Run();