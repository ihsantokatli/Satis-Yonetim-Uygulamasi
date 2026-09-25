# 🛒 Satış Yönetim Uygulaması (FSD_Project_01)

ASP.NET Core 8 MVC platformunda geliştirilmiş; firmaların ürün envanterlerini yönetmelerine, POS (Sepet) mantığıyla satış yapmalarına ve detaylı finansal raporlar almalarına olanak tanıyan kapsamlı bir otomasyon sistemidir.

## 🚀 Proje Özellikleri

*   **Yetkilendirme ve Kullanıcı Yönetimi (RBAC):** ASP.NET Core Identity altyapısı kullanılarak `CompanyAdmin` (Yönetici) ve `CompanyUser` (Kullanıcı) olmak üzere rol bazlı erişim kontrolü sağlanmıştır. Yöneticiler hesapları askıya alabilir (Soft-deactivate).
*   **Ürün ve Kategori Yönetimi:** Eager Loading mimarisiyle performanslı ürün listeleme, stok takibi ve ürün ekleme/güncelleme işlemleri.
*   **Dinamik Satış ve Sepet Modülü:** Sayfa yenilenmeden asenkron olarak çalışan (Fetch API & Session) gelişmiş sepet sistemi.
*   **Canlı Döviz Entegrasyonu:** TCMB (Türkiye Cumhuriyet Merkez Bankası) XML web servisi ile anlık Dolar (USD) ve Euro (EUR) kurlarının çekilip sepet tutarına yansıtılması.
*   **Stok Hareketleri Loglaması:** Satış anında veritabanı Transaction'ları ile stokların güvenli bir şekilde düşülmesi ve hareketlerin loglanması.
*   **Gelişmiş Raporlama:** LINQ kullanılarak hazırlanan, tarih filtreli ve ürün bazlı kâr/zarar analizleri.
*   **Excel Export ve DataTables:** Raporların ve listelerin hızlı aranabilmesi, filtrelenebilmesi ve Excel formatında dışa aktarılabilmesi.
*   **Yazdırılabilir Satış Fişi:** Satış sonrasında termal yazıcılara uygun formatta (print media query) otomatik fiş ekranı oluşturma.

## 🛠️ Kullanılan Teknolojiler

**Backend:**
*   C# & .NET 8 (ASP.NET Core MVC)
*   Entity Framework Core (Code-First Approach)
*   Microsoft SQL Server
*   Generic Repository Design Pattern
*   ASP.NET Core Identity

**Frontend:**
*   HTML5, CSS3, JavaScript (Fetch API)
*   Bootstrap 5 (Responsive UI)
*   jQuery & DataTables (Excel Export entegrasyonlu)

## 🗄️ Veritabanı Mimarisi
Proje **Code-First** yaklaşımıyla tasarlanmıştır. Temel tablolar:
`AppUser` (Kullanıcılar), `Product` (Ürünler), `Category` (Kategoriler), `Sale` (Satışlar), `SaleDetail` (Satış Detayları), `StockMovement` (Stok Hareketleri), `Currency` (Döviz).

## ⚙️ Kurulum ve Çalıştırma

Projeyi yerel ortamınızda (Local) çalıştırmak için aşağıdaki adımları izleyin:

1. **Depoyu Klonlayın:**
   ```bash
   git clone [https://github.com/ihsantokatli/Satis-Yonetim-Uygulamasi.git](https://github.com/ihsantokatli/Satis-Yonetim-Uygulamasi.git)

Proje varsayılan olarak LocalDB kullanacak şekilde yapılandırılmıştır. Farklı bir SQL Server kullanacaksanız appsettings.json içerisindeki DefaultConnection dizesini güncelleyin.

🔐 Varsayılan Giriş Bilgileri
Proje ilk ayağa kalktığında test edebilmeniz için otomatik oluşturulan Yönetici hesabı:

Kullanıcı Adı: admin

Şifre: Admin123!

👨‍💻 Geliştirici
İhsan TOKATLI

Fırat Üniversitesi - Bilgisayar Mühendisliği
