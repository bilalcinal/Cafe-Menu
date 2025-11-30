# ☕ Cafe Menu - Kafe Menü Yönetim Sistemi

Modern ve ölçeklenebilir bir kafe menü yönetim sistemi. .NET 8 ve ASP.NET Core MVC kullanılarak geliştirilmiş, Clean Architecture prensiplerine uygun olarak tasarlanmıştır.

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Mimari](#-mimari)
- [Kurulum](#-kurulum)
  - [Hızlı Başlangıç (Docker)](#hızlı-başlangıç-docker)
  - [Manuel Kurulum](#manuel-kurulum)
- [Kullanım Kılavuzu](#-kullanım-kılavuzu)
  - [Admin Panel](#admin-panel)
  - [Müşteri Panel](#müşteri-panel)
- [Proje Yapısı](#-proje-yapısı)
- [Önemli Özellikler](#-önemli-özellikler)
- [Geliştirme](#-geliştirme)

---

## ✨ Özellikler

### 🎯 Müşteri Paneli
- 📱 Responsive tasarım ile her cihazda mükemmel görünüm
- 🍽️ Kategori bazlı ürün listeleme ve görüntüleme
- 💱 Çoklu para birimi desteği (TRY, USD, EUR)
- 🔄 Gerçek zamanlı döviz kuru güncellemeleri
- 📋 Ürün özelliklerinin detaylı gösterimi
- 🏢 Multi-tenant desteği (her tenant kendi menüsünü görür)

### 🔐 Admin Paneli
- 📊 Dashboard ile istatistiksel görünüm
  - Kategoriye göre ürün sayısı widget'ı
  - Canlı döviz kurları widget'ı (10 saniyede bir otomatik güncelleme)
- 📦 **Kategori Yönetimi**: CRUD işlemleri
- 🍕 **Ürün Yönetimi**: CRUD işlemleri, görsel yükleme
- 🏷️ **Özellik (Property) Yönetimi**: Ürün özelliklerini tanımlama
- 🔗 **Ürün-Özellik Eşleştirme**: Ürünlere özellik atama
- 👥 **Kullanıcı Yönetimi**: Kullanıcı oluşturma, düzenleme, silme
- 🔑 **Rol ve İzin Yönetimi**: Detaylı yetkilendirme sistemi
- 🏢 **Tenant Yönetimi**: Multi-tenant yapı yönetimi

---

## 🛠️ Teknolojiler

- **.NET 8** - En son .NET framework
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core 8** - ORM
- **SQL Server** - Veritabanı
- **Bootstrap 5** - Frontend framework
- **Cookie Authentication** - Kimlik doğrulama
- **Memory Cache** - Performans optimizasyonu
- **Docker** - Containerization

---

## 🏗️ Mimari

Proje **Clean Architecture** prensiplerine göre 4 ana katmandan oluşmaktadır:

### 📦 CafeMenu.Domain
- Domain entities (Category, Product, Property, ProductProperty, User, Tenant, Role, Permission)
- Value objects ve enums
- Business rules ve domain logic
- **Hiçbir dış bağımlılık içermez** (Pure Domain Layer)

### 🔧 CafeMenu.Application
- Use case servisleri (CustomerMenuService, CategoryService, ProductService, vb.)
- Repository ve service interface'leri
- DTOs ve ViewModels
- Domain katmanına bağımlıdır

### 🔌 CafeMenu.Infrastructure
- EF Core DbContext ve entity configurations
- Repository implementasyonları
- External service implementasyonları (CurrencyService, ProductCacheService, MenuPdfService)
- Tenant resolver implementasyonu
- Stored procedure çağrıları (user authentication)
- Application ve Domain katmanlarına bağımlıdır

### 🌐 CafeMenu.Web
- ASP.NET Core MVC controllers
- Razor views
- Dependency injection configuration
- Application ve Infrastructure katmanlarına bağımlıdır

---

## 🚀 Kurulum

### Hızlı Başlangıç (Docker)

En kolay yol! Docker ve Docker Compose ile tek komutla başlatın:

```bash
# Projeyi klonlayın
git clone <repository-url>
cd Cafe-Menu

# Docker Compose ile tüm servisleri başlatın
docker-compose up -d

# Logları takip etmek için
docker-compose logs -f web
```

Uygulama şu adreslerde çalışacaktır:
- **Web Uygulaması**: http://localhost:8080
- **SQL Server**: localhost:1433

> ⚠️ **Not**: İlk çalıştırmada veritabanı migration'ları otomatik olarak uygulanacak ve seed data yüklenecektir. Bu işlem birkaç saniye sürebilir.

### Manuel Kurulum

#### Gereksinimler

- .NET 8 SDK ([İndir](https://dotnet.microsoft.com/download/dotnet/8.0))
- SQL Server 2022 veya üzeri
  - Alternatif: SQL Server Express veya Docker ile SQL Server
- Visual Studio 2022, VS Code veya herhangi bir IDE

#### Adım 1: Projeyi Klonlayın

```bash
git clone <repository-url>
cd Cafe-Menu
```

#### Adım 2: SQL Server'ı Başlatın

**Docker ile SQL Server (Önerilen):**

```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Strong!Pass2025" \
  -p 1433:1433 \
  --name cafe-menu-sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

**Veya mevcut SQL Server kullanıyorsanız:**
- SQL Server Management Studio (SSMS) ile bağlanın
- Yeni bir veritabanı oluşturun (opsiyonel, migration otomatik oluşturur)

#### Adım 3: Connection String'i Yapılandırın

`src/CafeMenu.Web/appsettings.json` dosyasını açın ve connection string'i düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CafeMenuDb;User Id=sa;Password=Strong!Pass2025;TrustServerCertificate=True;"
  }
}
```

> 💡 **İpucu**: Eğer farklı bir SQL Server kullanıyorsanız, connection string'i buna göre güncelleyin.

#### Adım 4: Stored Procedure'ları Oluşturun

SQL Server Management Studio veya Azure Data Studio ile bağlanın ve şu dosyaları çalıştırın:

1. `src/CafeMenu.Infrastructure/Scripts/sp_CreateUser.sql`
2. `src/CafeMenu.Infrastructure/Scripts/sp_ValidateUser.sql`

Bu stored procedure'lar kullanıcı şifrelerini hash+salt ile saklamak için kullanılır.

#### Adım 5: Veritabanı Migration'larını Uygulayın

```bash
cd src/CafeMenu.Web

# Migration'ları uygula (veritabanı otomatik oluşturulur)
dotnet ef database update --project ../CafeMenu.Infrastructure
```

> 📝 **Not**: İlk çalıştırmada seed data otomatik olarak yüklenecektir (Program.cs içinde).

#### Adım 6: Uygulamayı Çalıştırın

```bash
# Development modunda çalıştır
dotnet run

# Veya
dotnet watch run  # Hot reload ile
```

Uygulama şu adreslerde çalışacaktır:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

> 🔒 **Güvenlik**: İlk çalıştırmada SSL sertifikası uyarısı alabilirsiniz. Development ortamında "Gelişmiş" > "Devam Et" ile geçebilirsiniz.

---

## 📖 Kullanım Kılavuzu

### Admin Panel

#### Giriş Yapma

1. Tarayıcınızda şu adrese gidin: `https://localhost:5001/Admin/Account/Login?tenantId=1`
2. Varsayılan admin bilgileri:
   - **Kullanıcı Adı**: `admin`
   - **Şifre**: `Admin123!`
   - **Tenant ID**: `1` (URL'de belirtilmiş)

> ⚠️ **Güvenlik Uyarısı**: İlk girişten sonra mutlaka şifreyi değiştirin!

#### Dashboard

Giriş yaptıktan sonra admin dashboard'a yönlendirileceksiniz. Burada:
- 📊 Kategoriye göre ürün sayısı grafiği
- 💱 Canlı döviz kurları (TRY, USD, EUR)
- 📈 Sistem istatistikleri

#### Kategori Yönetimi

1. Sol menüden **"Kategoriler"** seçeneğine tıklayın
2. **"Yeni Kategori"** butonu ile yeni kategori ekleyin
3. Mevcut kategorileri düzenleyebilir veya silebilirsiniz

#### Ürün Yönetimi

1. Sol menüden **"Ürünler"** seçeneğine tıklayın
2. **"Yeni Ürün"** butonu ile yeni ürün ekleyin
3. Ürün bilgilerini girin:
   - Ürün adı
   - Açıklama
   - Fiyat (TRY cinsinden)
   - Kategori seçimi
   - Ürün görseli (opsiyonel)
4. Ürün özelliklerini eklemek için **"Özellikler"** sekmesine gidin

#### Özellik (Property) Yönetimi

1. Sol menüden **"Özellikler"** seçeneğine tıklayın
2. Yeni özellik ekleyin (örn: "Boyut", "İçecek Tipi", "Sıcaklık")
3. Ürünlere bu özellikleri atayın

#### Kullanıcı Yönetimi

1. Sol menüden **"Kullanıcılar"** seçeneğine tıklayın
2. Yeni kullanıcı oluşturun
3. Kullanıcılara rol atayın
4. Kullanıcıları düzenleyebilir veya silebilirsiniz

#### Rol ve İzin Yönetimi

1. Sol menüden **"Roller"** seçeneğine tıklayın
2. Yeni rol oluşturun
3. Rollere izin atayın
4. Kullanıcıları rollere atayın

### Müşteri Panel

#### Menüyü Görüntüleme

1. Tarayıcınızda şu adrese gidin: `https://localhost:5001/Customer?tenantId=1`
2. Kategorilere göre ürünleri görüntüleyin
3. Para birimini değiştirmek için sağ üstteki para birimi seçicisini kullanın
4. Ürün detaylarını görmek için ürün kartına tıklayın

#### Özellikler

- **Çoklu Para Birimi**: TRY, USD, EUR arasında geçiş yapabilirsiniz
- **Otomatik Kur Güncellemesi**: Döviz kurları otomatik olarak güncellenir
- **Responsive Tasarım**: Mobil, tablet ve masaüstünde mükemmel görünüm
- **Kategori Filtreleme**: Kategorilere göre ürünleri filtreleyin

---

## 📁 Proje Yapısı

```
Cafe-Menu/
├── src/
│   ├── CafeMenu.Domain/              # Domain Layer
│   │   ├── Entities/                 # Domain entities
│   │   ├── Enums/                    # Enumeration types
│   │   └── ValueObjects/             # Value objects
│   │
│   ├── CafeMenu.Application/         # Application Layer
│   │   ├── Interfaces/
│   │   │   ├── Repositories/         # Repository interfaces
│   │   │   └── Services/             # Service interfaces
│   │   ├── Models/                   # DTOs ve ViewModels
│   │   └── Services/                 # Application services
│   │
│   ├── CafeMenu.Infrastructure/      # Infrastructure Layer
│   │   ├── Migrations/               # EF Core migrations
│   │   ├── Persistence/              # DbContext ve configurations
│   │   ├── Repositories/             # Repository implementations
│   │   ├── Scripts/                  # SQL stored procedures
│   │   ├── SeedData/                 # Seed data classes
│   │   └── Services/                 # Infrastructure services
│   │
│   └── CafeMenu.Web/                 # Presentation Layer
│       ├── Areas/
│       │   └── Admin/                 # Admin area (controllers, views)
│       ├── Controllers/               # MVC controllers
│       ├── Views/                     # Razor views
│       ├── wwwroot/                   # Static files (CSS, JS, images)
│       └── Program.cs                 # Application entry point
│
├── docker-compose.yml                 # Docker Compose configuration
├── Dockerfile                         # Docker image definition
└── README.md                          # Bu dosya
```

---

## 🔑 Önemli Özellikler

### 🔐 Güvenlik

- **Hash + Salt**: Kullanıcı şifreleri SQL Server stored procedure'ları ile SHA2-256 hash ve random salt kullanılarak saklanır
- **Cookie Authentication**: Güvenli cookie tabanlı kimlik doğrulama
- **Permission-Based Authorization**: Detaylı izin sistemi ile yetkilendirme

### 🏢 Multi-Tenancy

Sistem tam multi-tenant yapıdadır. Her tenant kendi verilerini görür:

- Tenant ID query string parametresi ile belirlenir: `?tenantId=1`
- Tüm repository metodları otomatik tenant filtrelemesi yapar
- Varsayılan tenant ID: `1` (belirtilmezse)

**Örnek URL'ler:**
```
/Customer?tenantId=1
/Admin/Category?tenantId=1
/Admin/Dashboard?tenantId=1
```

### ⚡ Performans Optimizasyonu

**Cache Stratejisi:**
- 5 milyon ürün kaydını verimli yönetmek için tenant-aware in-memory cache
- Her tenant için ayrı cache key: `products_tenant_{tenantId}`
- Cache expiration: 30 dakika
- Ürün güncellemelerinde ilgili tenant cache'i invalidate edilir
- Sadece aktif (IsDeleted=false) ürünler cache'lenir

**Avantajlar:**
- ✅ Memory kullanımını optimize eder
- ✅ Performansı artırır (her istekte DB sorgusu yapılmaz)
- ✅ Ölçeklenebilir (distributed cache'e geçiş kolaydır)

### 💱 Döviz Kuru Sistemi

- Gerçek zamanlı döviz kuru çekme (external API)
- 10 saniyede bir otomatik güncelleme
- TRY, USD, EUR desteği
- Cache ile API çağrılarını minimize eder

### 📄 PDF Menü Export

- Menüyü PDF olarak export etme özelliği
- Müşteri panelinden PDF indirme

---

## 🛠️ Geliştirme

### Yeni Migration Oluşturma

```bash
cd src/CafeMenu.Web
dotnet ef migrations add MigrationAdi --project ../CafeMenu.Infrastructure
```

### Migration'ları Uygulama

```bash
dotnet ef database update --project ../CafeMenu.Infrastructure
```

### Migration'ı Geri Alma

```bash
dotnet ef database update PreviousMigrationName --project ../CafeMenu.Infrastructure
```

### Docker Container'ları Yönetme

```bash
# Container'ları durdur
docker-compose down

# Container'ları durdur ve volume'ları sil
docker-compose down -v

# Container'ları yeniden başlat
docker-compose restart

# Logları görüntüle
docker-compose logs -f

# Belirli bir servisin loglarını görüntüle
docker-compose logs -f web
docker-compose logs -f sqlserver
```

### Test Kullanıcıları

İlk çalıştırmada otomatik olarak oluşturulan varsayılan kullanıcı:

- **Kullanıcı Adı**: `admin`
- **Şifre**: `Admin123!`
- **Rol**: SuperAdmin
- **Tenant ID**: 1

---

## 📝 Notlar

- Stored procedure'lar (`sp_CreateUser`, `sp_ValidateUser`) veritabanında oluşturulmalıdır
- İlk çalıştırmada seed data otomatik yüklenir (Tenant, Role, Permission, User)

---

## 📄 Lisans

Bu proje bir teknik case çalışmasıdır.

