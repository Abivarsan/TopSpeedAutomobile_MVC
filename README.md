# 🏁 TopSpeed Automobile

<div align="center">

![TopSpeed Banner](TopSpeed.Web/wwwroot/images/brand/ferrari.png)

### **Next-Generation Automotive Fleet & Performance Showcase Platform**

An enterprise-grade **ASP.NET Core 8.0 MVC** web application engineered for hypercars, luxury vehicle manufacturers, virtual garage curation, and side-by-side spec comparisons.

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet)
[![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Entity Framework Core](https://img.shields.io/badge/ORM-EF_Core_8.0-512BD4?style=for-the-badge&logo=nuget&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![Bootstrap 5](https://img.shields.io/badge/UI-Bootstrap_5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![Chart.js](https://img.shields.io/badge/Analytics-Chart.js-FF6384?style=for-the-badge&logo=chartdotjs&logoColor=white)](https://www.chartjs.org/)

</div>

---

## 🚀 Key Features

### 🏎️ 1. Vehicle Showroom & Fleet Management
- **Full Technical Dossiers**: Comprehensive tracking of Horsepower, 0–100 km/h acceleration, Top Speed, Powertrain, Transmission, Model Year, and Valuation.
- **Dynamic Showroom Filters**: Instant filtering by vehicle categories (*Hypercar, Supercar, Sports Coupe, Sedan, SUV, EV*), manufacturer selection, and sorting by horsepower or price.
- **Admin Fleet Operations**: Drag-and-drop vehicle photo uploads, real-time image previews, and full CRUD capabilities.

### ⚔️ 2. Head-to-Head "Spec Battle"
- Compare any two performance machines side-by-side.
- Animated **Tale of the Tape** comparative power & velocity meters.
- Automatic **"Winner"** badge allocation for superior acceleration, horsepower, and top speed metrics.

### 📊 3. Role-Based Access & Admin Analytics
- **Multi-Role Security**: Powered by **ASP.NET Core Identity** with role-based policies (`Admin` vs `Customer`).
- **Interactive Chart.js Dashboard**:
  - Fleet Horsepower tier distribution bar chart.
  - Manufacturer catalog market-share donut chart.
  - Real-time KPI summary tiles (Total Fleet, Active Brands, Inquiries, Combined Horsepower).

### 🏆 4. Customer "Virtual Garage"
- Authenticated users can bookmark and curate their dream automotive garage.
- Aggregated garage analytics calculating **Combined Horsepower**, **Average Top Speed**, and **Total Fleet Valuation**.

### 🏷️ 5. Extended Brand Lineage
- Rich manufacturer dossiers displaying founding year, country of origin, founder, global headquarters, and verified website links.
- Heritage biography storytelling and catalog linking all vehicles registered to each marque.

### 📅 6. Test Drive Concierge & Automated SMTP
- Seamless test-drive reservation form highlighting the selected machine's performance figures.
- Automated email dispatch via SMTP upon customer booking confirmation.
- Admin inquiry management panel to review, confirm, and complete reservations.

### 📄 7. CSV Export & Reporting
- One-click CSV reporting endpoints for both **Manufacturers** (`/Brand/ExportCsv`) and **Vehicle Fleets** (`/Vehicle/ExportCsv`).

### 🎨 8. "Hyper Dark & Crimson Speed" Design System
- Modern glassmorphism UI styled with `backdrop-filter: blur()`, glowing borders, and speed red accents.
- Responsive mobile-first layout with custom DataTables dark integration and Toastr.js alert toasts.

---

## 🛠️ Technology Stack

| Layer | Technologies |
| :--- | :--- |
| **Framework** | ASP.NET Core 8.0 (MVC Architecture) |
| **Language** | C# 12 |
| **Database & ORM** | Microsoft SQL Server, Entity Framework Core 8.0 (Code-First) |
| **Authentication** | ASP.NET Core Identity (Role-Based Authorization) |
| **Front-End Styling** | Bootstrap 5, Bootstrap Icons, Custom Glassmorphism CSS3 |
| **Client Scripting** | jQuery, DataTables, Chart.js, Toastr.js |
| **Mailing Service** | MailKit / MimeKit SMTP with secure Google App Passwords |

---

## 📋 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server) or [SQL Server Express / LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb)
- [EF Core CLI Tool](https://learn.microsoft.com/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)

---

### Installation & Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/Abivarsan/TopSpeedAutomobile_MVC.git
   cd TopSpeedAutomobile_MVC
   ```

2. **Configure Connection String**
   Open `TopSpeed.Web/appsettings.json` and adjust your SQL Server connection string if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TopSpeeedWebDB;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=True"
   }
   ```

3. **Configure SMTP Mail Settings (Optional for Email Dispatch)**
   In `TopSpeed.Web/appsettings.json`:
   ```json
   "EmailSettings": {
     "FromEmail": "your-email@gmail.com",
     "Password": "your-app-password",
     "Host": "smtp.gmail.com",
     "Port": 587
   }
   ```

4. **Apply Database Migrations**
   ```bash
   dotnet ef database update --project TopSpeed.Web
   ```

5. **Run the Application**
   ```bash
   dotnet run --project TopSpeed.Web
   ```
   Navigate to `http://localhost:5110` in your web browser.

---

## 🔑 Default Administrator & Database Seeding

On initial launch, [`DbInitializer`](TopSpeed.Web/Data/DbInitializer.cs) automatically executes and seeds:
- **Roles**: `Admin` and `Customer`.
- **Admin Assignment**: Grants `Admin` role to the primary developer account `ketheeswaranabivarsan@gmail.com`.
- **Iconic Brands**: Ferrari, Porsche, Lamborghini, McLaren, Nissan.
- **Hypercars**: Ferrari SF90 Stradale, Ferrari 296 GTB, Porsche 918 Spyder, Porsche 911 GT3 RS, Lamborghini Revuelto, McLaren 750S, Nissan GT-R Nismo.

---

## 📂 Project Structure

```text
TopSpeedAutomobile/
├── TopSpeed.Web/
│   ├── Areas/
│   │   └── Identity/             # ASP.NET Identity Auth Pages (Login, Register, Forgot Password)
│   ├── Controllers/
│   │   ├── AdminController.cs    # Analytics & Inquiry Review Panel
│   │   ├── BrandController.cs    # Manufacturer Management & CSV Export
│   │   ├── GarageController.cs   # Virtual Garage Bookmarks & Aggregations
│   │   ├── HomeController.cs     # Automotive Showcase Landing
│   │   ├── InquiryController.cs  # Test Drive Booking & Dispatch
│   │   └── VehicleController.cs  # Showroom, Spec Battle & Fleet CRUD
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── DbInitializer.cs      # Database Seeder (Roles & Vehicles)
│   ├── Migrations/               # EF Core Migrations
│   ├── Models/
│   │   ├── ApplicationUser.cs
│   │   ├── Brand.cs              # Manufacturer Entity
│   │   ├── Inquiry.cs            # Test Drive Reservation Entity
│   │   ├── UserGarageItem.cs     # Virtual Garage Join Entity
│   │   ├── Vehicle.cs            # Vehicle Specification Entity
│   │   └── ViewModels/
│   ├── Services/
│   │   └── EmailSender.cs        # SMTP Mail Service
│   ├── Views/
│   │   ├── Admin/                # Analytics Dashboard & Inquiries Views
│   │   ├── Brand/                # Catalog, Dossier, CRUD Views
│   │   ├── Garage/               # Personal Virtual Garage Views
│   │   ├── Inquiry/              # Booking Interface Views
│   │   ├── Vehicle/              # Showroom, Details, Compare, CRUD Views
│   │   └── Shared/               # Layout, Navbar, Footers & Partials
│   └── wwwroot/
│       ├── css/site.css          # Hyper Dark & Crimson Speed Theme
│       ├── images/               # Brand Logos & Vehicle Photography
│       └── js/site.js
├── README.md
└── TopSpeedAutomobile.sln
```

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).

Developed with ❤️ for high-performance automotive engineering.
