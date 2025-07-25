# ZOL Shop - Visual Studio Setup Guide

## Opening in Visual Studio

### Method 1: Open Solution File
1. Double-click `ZOLShop.sln` to open in Visual Studio
2. Visual Studio will automatically restore NuGet packages

### Method 2: Open Folder
1. Open Visual Studio
2. File → Open → Folder
3. Select the `ZOLShop` folder

## Visual Studio Configuration

### 1. Set Startup Project
- Right-click `ZOLShop` project → Set as Startup Project

### 2. Configure Debug Settings
- Project Properties → Debug → Launch Profiles
- Use `ZOLShop` profile for development
- Use `IIS Express` for web server testing

### 3. Package Manager Console
- Tools → NuGet Package Manager → Package Manager Console
- Run Entity Framework commands:
```
Add-Migration InitialCreate
Update-Database
```

## Running the Application

### Debug Mode (F5)
- Press F5 or click Start Debugging
- Application opens at https://localhost:5001

### Without Debugging (Ctrl+F5)
- Press Ctrl+F5 or click Start Without Debugging
- Faster startup, no debugging overhead

## Visual Studio Features

### IntelliSense & Code Completion
- Full C# and Razor syntax support
- Auto-completion for models and controllers

### Debugging
- Set breakpoints in controllers and views
- Step through code execution
- Watch variables and expressions

### Package Management
- Right-click project → Manage NuGet Packages
- Update packages through GUI

### Database Tools
- View → SQL Server Object Explorer
- Connect to (localdb)\mssqllocaldb
- Browse ZOLShopDB tables

## Project Structure in Solution Explorer

```
ZOLShop/
├── Controllers/          # MVC Controllers
├── Data/                # DbContext and migrations
├── Models/              # Entity models
├── Services/            # Email and other services
├── Views/               # Razor views
│   ├── Account/         # Authentication views
│   ├── Admin/           # Admin panel views
│   ├── Cart/            # Shopping cart views
│   ├── Home/            # Main product views
│   ├── Payment/         # Checkout views
│   └── Shared/          # Layout and shared views
├── wwwroot/             # Static files
│   ├── css/             # Stylesheets
│   ├── js/              # JavaScript files
│   └── images/          # Product and profile images
├── Properties/          # Launch settings
├── appsettings.json     # Configuration
└── Program.cs           # Application entry point
```

## Useful Visual Studio Extensions

### Recommended Extensions:
- **Web Essentials** - Enhanced web development
- **Productivity Power Tools** - Additional VS features
- **CodeMaid** - Code cleanup and organization
- **Resharper** - Advanced code analysis (paid)

## Troubleshooting

### Build Errors
1. Clean Solution (Build → Clean Solution)
2. Rebuild Solution (Build → Rebuild Solution)
3. Restore NuGet packages (right-click solution → Restore NuGet Packages)

### Database Issues
1. Delete Migrations folder
2. Run `Add-Migration InitialCreate` in Package Manager Console
3. Run `Update-Database`

### Port Conflicts
- Change ports in Properties/launchSettings.json
- Use different ports if 5000/5001 are occupied

## Demo Accounts
- **Admin**: admin@zolshop.com / Admin123!
- **Customer**: customer1@zolshop.com / Customer123!

## Email Configuration
Update appsettings.json with your Gmail credentials:
```json
"EmailSettings": {
  "FromEmail": "your-email@gmail.com",
  "Username": "your-email@gmail.com",
  "Password": "your-app-password"
}
```