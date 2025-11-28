# Cafe Menu Project

.NET 8 ve ASP.NET Core MVC kullanılarak geliştirilmiş, Clean Architecture prensiplerine uygun bir kafe menü yönetim sistemi.

## Mimari

Proje Clean Architecture prensiplerine göre 4 ana katmandan oluşmaktadır:

### CafeMenu.Domain
- Domain entities (Category, Product, Property, ProductProperty, User, Tenant)
- Value objects ve enums
- Business rules ve domain logic
- Hiçbir dış bağımlılık içermez

### CafeMenu.Application
- Use case servisleri (CustomerMenuService, CategoryService, ProductService, vb.)
- Repository ve service interface'leri
- DTOs ve ViewModels
- Domain katmanına bağımlıdır

### CafeMenu.Infrastructure
- EF Core DbContext ve entity configurations
- Repository implementasyonları
- External service implementasyonları (CurrencyService, ProductCacheService)
- Tenant resolver implementasyonu
- Stored procedure çağrıları (user authentication)
- Application ve Domain katmanlarına bağımlıdır

### CafeMenu.Web
- ASP.NET Core MVC controllers
- Razor views
- Dependency injection configuration
- Application ve Infrastructure katmanlarına bağımlıdır

## Özellikler

### Customer Panel
- Public menü ekranı
- Kategori bazlı ürün listeleme
- Çoklu para birimi desteği (TRY, USD, EUR)
- Ürün özelliklerinin gösterimi
- Multi-tenant desteği

### Admin Panel
- Kategori CRUD işlemleri
- Ürün CRUD işlemleri
- Özellik (Property) CRUD işlemleri
- Ürün-Özellik eşleştirme
- Kullanıcı yönetimi (stored procedure ile hash+salt)
- Dashboard:
  - Kategoriye göre ürün sayısı widget'ı
  - Döviz kurları widget'ı (10 saniyede bir otomatik güncelleme)

## Teknolojiler

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server
- Bootstrap 5
- Cookie Authentication

## Veritabanı

SQL Server kullanılmaktadır. Connection string `appsettings.json` dosyasında yapılandırılabilir.

### Stored Procedures

Proje, kullanıcı şifrelerini hash+salt ile saklamak için stored procedure'lar kullanır:

- `sp_CreateUser`: Yeni kullanıcı oluşturur ve şifreyi hash+salt ile saklar
- `sp_ValidateUser`: Kullanıcı girişini doğrular

Bu stored procedure'ları veritabanında çalıştırmanız gerekmektedir. SQL script'leri `src/CafeMenu.Infrastructure/Scripts/` klasöründe bulunmaktadır.

## Cache Stratejisi

5 milyon ürün kaydını verimli bir şekilde yönetmek için tenant-aware in-memory cache stratejisi kullanılmaktadır:

- Her tenant için ayrı cache key kullanılır: `products_tenant_{tenantId}`
- Cache expiration: 30 dakika
- Ürün güncellemelerinde ilgili tenant cache'i invalidate edilir
- Sadece aktif (IsDeleted=false) ürünler cache'lenir
- Cache'de sadece gerekli alanlar (ProductDto) saklanır, tüm entity'ler değil

Bu yaklaşım:
- Memory kullanımını optimize eder (sadece ilgili tenant'ın verileri yüklenir)
- Performansı artırır (her istekte DB sorgusu yapılmaz)
- Ölçeklenebilir (distributed cache'e geçiş kolaydır)

## Multi-Tenancy

Sistem multi-tenant yapıdadır. Tenant ID query string parametresi ile belirlenir:

```
/Customer?tenantId=1
/Admin/Category?tenantId=1
```

Eğer tenantId belirtilmezse, varsayılan olarak tenant ID = 1 kullanılır.

Tüm repository metodları otomatik olarak tenant filtrelemesi yapar.

## Kurulum

### Gereksinimler

- .NET 8 SDK
- SQL Server (Docker'da çalıştırılabilir)
- Visual Studio 2022 veya VS Code

### Adımlar

1. Repository'yi klonlayın:
```bash
git clone <repository-url>
cd Cafe-Menu
```

2. SQL Server'ı başlatın (Docker kullanıyorsanız):
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Strong!Pass2025" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

3. Connection string'i `appsettings.json` dosyasında kontrol edin.

4. Stored procedure'ları çalıştırın:
```sql
-- sp_CreateUser.sql ve sp_ValidateUser.sql dosyalarını SQL Server'da çalıştırın
```

5. EF Core migrations oluşturun ve uygulayın:
```bash
cd src/CafeMenu.Web
dotnet ef migrations add InitialCreate --project ../CafeMenu.Infrastructure
dotnet ef database update
```

6. Uygulamayı çalıştırın:
```bash
dotnet run
```

7. Tarayıcıda açın: `https://localhost:5001` veya `http://localhost:5000`

## CI/CD

Proje GitHub Actions ile CI/CD pipeline'ı destekler. Örnek workflow dosyası `.github/workflows/ci.yml` içinde bulunmaktadır.

### Pipeline Adımları

1. Restore dependencies
2. Build solution
3. Run tests (eğer test projesi varsa)
4. Publish application
5. Build Docker image
6. Push to container registry

### Docker

Uygulama Docker container olarak çalıştırılabilir. `Dockerfile` dosyası proje kök dizininde bulunmaktadır.

Docker image oluşturma:
```bash
docker build -t cafe-menu:latest .
```

Docker container çalıştırma:
```bash
docker run -p 8080:80 -e ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=CafeMenuDb;User Id=sa;Password=Strong!Pass2025;TrustServerCertificate=True;" cafe-menu:latest
```

## Proje Yapısı

```
Cafe-Menu/
├── src/
│   ├── CafeMenu.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── ValueObjects/
│   ├── CafeMenu.Application/
│   │   ├── Interfaces/
│   │   ├── Models/
│   │   └── Services/
│   ├── CafeMenu.Infrastructure/
│   │   ├── Persistence/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── Scripts/
│   └── CafeMenu.Web/
│       ├── Areas/
│       │   └── Admin/
│       ├── Controllers/
│       ├── Views/
│       └── wwwroot/
├── .github/
│   └── workflows/
├── Dockerfile
└── README.md
```

## Lisans

Bu proje bir teknik case çalışmasıdır.

