@echo off
echo Setting up ZOL Shop Database...
echo.

echo Installing Entity Framework tools...
dotnet tool install --global dotnet-ef

echo.
echo Adding initial migration...
dotnet ef migrations add InitialCreate

echo.
echo Updating database...
dotnet ef database update

echo.
echo Database setup complete!
echo.
echo Demo accounts:
echo Admin: admin@zolshop.com / Admin123!
echo Customer: customer1@zolshop.com / Customer123!
echo.
echo Run 'dotnet run' to start the application.
pause

# Database Setup Guide

## Step 1: Install Entity Framework Tools
```bash
dotnet tool install --global dotnet-ef
```

## Step 2: Create Database Migration
```bash
cd c:\ZOLShop
dotnet ef migrations add InitialCreate
```

## Step 3: Update Database
```bash
dotnet ef database update
```

## Step 4: Run Application
```bash
dotnet run
```

## Demo Accounts
- **Admin**: admin@zolshop.com / Admin123!
- **Customer**: customer1@zolshop.com / Customer123!

## Connection String
Update `appsettings.json` if using different SQL Server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ZOLShopDB;Trusted_Connection=true"
}
```