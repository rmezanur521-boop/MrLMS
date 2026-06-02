# 📚 MrLMS — Library Management System

A clean and lightweight **Library Management System** built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core**, and **SQL Server**. It uses **session-based authentication** (no Identity dependency), custom CSS styling, and a straightforward MVC architecture for managing books, members, and library operations.

---

## 🚀 Tech Stack

| Layer          | Technology                          |
|----------------|--------------------------------------|
| Framework      | ASP.NET Core MVC (.NET 8)            |
| Language       | C#                                   |
| ORM            | Entity Framework Core 8              |
| Database       | SQL Server                           |
| Authentication | Custom session-based login           |
| UI             | Razor Views, HTML, CSS, JavaScript   |
| Session        | ASP.NET Core Distributed Memory Cache|

---

## 📁 Project Structure
MrLMS/
├── Controllers/          # MVC Controllers (Home, Books, Members, etc.)
├── Data/
│   └── AppDbContext.cs   # EF Core database context
├── Helper/               # Session helpers & utility classes
├── Models/               # Entity/domain models
├── Views/                # Razor Views for each controller
├── wwwroot/              # Static files (CSS, JS, images)
├── Properties/           # Launch settings
├── Program.cs            # App entry point & service registration
├── appsettings.json      # Configuration & DB connection string
└── MrLMS.csproj          # Project dependencies

---

## ✅ Features

### 🔐 Authentication
- Custom **session-based login** system (no ASP.NET Identity)
- Sessions expire after **30 minutes** of inactivity
- HttpOnly, essential cookies for security

### 📖 Library Operations
- Book management — add, edit, delete, and view books
- Member management — register and manage library members
- Book issue and return tracking
- Search and filter books by category or title

### 🎨 UI & Design
- Custom CSS styling (19% of codebase)
- Clean, responsive Razor Views
- Lightweight with minimal JavaScript

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup Steps

```bash
# 1. Clone the repository
git clone https://github.com/rmezanur521-boop/MrLMS.git
cd MrLMS
```

**2. Update connection string** in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=MrLMSDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

```bash
# 3. Apply database migrations
dotnet ef database update

# 4. Run the application
dotnet run
```

App will be available at: **https://localhost:7xxx** (shown in terminal)

---

## 📦 NuGet Packages

| Package | Version |
|---|---|
| Microsoft.EntityFrameworkCore | 8.0.26 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.26 |
| Microsoft.EntityFrameworkCore.Tools | 8.0.26 |

---

## 🗺️ Key Routes

| Path | Description |
|---|---|
| `/` | Home / Dashboard |
| `/Home/Index` | Landing page |
| `/Books` | Book listing & management |
| `/Members` | Member listing & management |

---

## 🔒 Session Configuration

Sessions are configured with the following settings in `Program.cs`:

```csharp
options.IdleTimeout = TimeSpan.FromMinutes(30);
options.Cookie.HttpOnly = true;
options.Cookie.IsEssential = true;
```

---

## 👤 Author

**Mezanur Rahman**
GitHub: [@rmezanur521-boop](https://github.com/rmezanur521-boop)

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).
