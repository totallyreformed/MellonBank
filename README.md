# MellonBank - E-Banking Platform

An e-banking management platform built with ASP.NET Core MVC (.NET 10.0) as part of the .NET educational program. The application provides a staff administration panel for managing customers and bank accounts, and a customer portal for performing banking operations.

## Technologies

- ASP.NET Core MVC (.NET 10.0)
- Entity Framework Core (Code-First)
- ASP.NET Core Identity
- SQL Server (LocalDB)
- Razor Views with Bootstrap
- Fixer.io API (currency conversion)

## Features

### Staff Panel (Admin)
- Add, edit, and delete customers (lookup by AFM)
- Add, edit, and delete bank accounts (lookup by Account Number)
- Search for a customer by AFM
- View all registered customers with their details

### Customer Portal
- View all personal bank accounts
- Check account balance in EUR and USD (live exchange rate via fixer.io)
- Deposit funds to own account
- Transfer funds to a third-party account (with balance validation)
- View account details
- Change personal password

### Authentication & Authorization
- Role-based access control (Staff / Customer) via ASP.NET Core Identity
- Staff and Customer roles are seeded automatically on application startup
- Registration with role selection and full profile fields (Name, Last Name, Address, Phone, Email, AFM)
- Login / Logout with Identity Razor Pages

## Project Structure

```
MellonBank/
├── Areas/Identity/
│   ├── Data/
│   │   ├── AppUser.cs              # Custom IdentityUser (Name, LastName, Address, AFM)
│   │   ├── Account.cs              # Bank account model
│   │   ├── AccountType.cs          # Enum (Checking, Savings)
│   │   └── AppDBContext.cs         # IdentityDbContext with Accounts DbSet
│   └── Pages/Account/
│       ├── Register.cshtml/.cs     # Registration with role selection
│       ├── Login.cshtml/.cs        # Login page
│       └── Logout.cshtml/.cs       # Logout page
├── Controllers/
│   ├── HomeController.cs           # Home and Privacy pages
│   ├── StaffController.cs          # Staff admin panel (Authorize: Staff)
│   └── CustomerController.cs       # Customer portal (Authorize: Customer)
├── ViewModels/
│   ├── CustomerViewModel.cs        # Create customer form
│   ├── EditCustomerViewModel.cs    # Edit customer form
│   ├── AccountViewModel.cs         # Create/Edit account form
│   ├── DepositViewModel.cs         # Deposit form
│   ├── TransferViewModel.cs        # Transfer form
│   └── ChangePasswordViewModel.cs  # Change password form
├── Models/
│   └── ErrorViewModel.cs
├── Views/
│   ├── Staff/                      # 10 views for staff operations
│   ├── Customer/                   # 6 views for customer operations
│   ├── Home/                       # Index, Privacy
│   └── Shared/                     # _Layout, _LoginPartial, Error
├── Program.cs                      # App configuration, DI, role seeding
└── appsettings.json                # Connection string, Fixer API key
```

## Prerequisites

- .NET 10.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022+ (recommended)
- Fixer.io API key (free tier: https://fixer.io/)

## Setup

1. Clone the repository
   ```
   git clone https://github.com/YOUR_USERNAME/MellonBank.git
   ```

2. Update the connection string in `appsettings.json` if needed
   ```json
   "ConnectionStrings": {
       "AppDBContextConnection": "Server=(localdb)\\mssqllocaldb;Database=MellonBank;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. Add your fixer.io API key in `appsettings.json`
   ```json
   "FixerApi": {
       "ApiKey": "your_api_key_here"
   }
   ```

4. Apply migrations
   ```
   Update-Database
   ```

5. Run the application
   ```
   dotnet run
   ```

6. Register a user and select a role (Staff or Customer) to get started

## Database Schema

- **AspNetUsers** — Stores user data (Name, LastName, Address, Phone, Email, AFM, credentials) via ASP.NET Core Identity
- **AspNetRoles** — Stores roles (Staff, Customer), seeded on startup
- **Accounts** — Stores bank accounts (AccountNumber, Balance, Branch, Type) with FK to AspNetUsers
