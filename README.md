# ZOL E-commerce Shop

A comprehensive ASP.NET Core MVC e-commerce application with full shopping cart functionality, user authentication, and admin management.

## Features

### Core Functionality
- **User Authentication**: Registration and login with ASP.NET Core Identity
- **Product Management**: Browse products with search and filtering
- **Shopping Cart**: Add, update, remove items with real-time updates
- **Order Management**: Complete checkout process with order tracking
- **Admin Panel**: Full CRUD operations for products, users, and orders

### Database Design
- **Products**: ProductID, Name, Description, Price, Stock, CategoryID, Image
- **Categories**: CategoryID, Name, Description
- **Users**: Extended ApplicationUser with FullName, Address
- **Carts**: CartID, CustomerID, CreatedAt
- **CartDetails**: CartDetailID, CartID, ProductID, Quantity, Price
- **Orders**: OrderID, CustomerID, OrderDate, TotalAmount, Status, ShippingAddress
- **Reviews**: ReviewID, ProductID, CustomerID, Rating, Comment, ReviewDate

### Sample Data
- 15 products across 2 categories (Electronics, Clothing)
- 3 pre-configured users (1 admin, 2 customers)
- Demo accounts for testing

## Tech Stack
- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: HTML5, CSS3, Bootstrap 5, JavaScript
- **Icons**: Font Awesome

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Navigate to the project directory
3. Update the connection string in `appsettings.json` if needed
4. Run the following commands:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

### Demo Accounts
- **Admin**: admin@zolshop.com / Admin123!
- **Customer**: customer1@zolshop.com / Customer123!
- **Customer**: customer2@zolshop.com / Customer123!

## Project Structure
```
ZOLShop/
├── Controllers/          # MVC Controllers
├── Data/                # DbContext and seed data
├── Models/              # Entity models
├── Views/               # Razor views
├── wwwroot/             # Static files (CSS, JS, images)
└── Program.cs           # Application entry point
```

## Key Features Implementation

### Search & Filtering
- Product name and description search
- Category-based filtering
- Price range filtering
- Real-time search results

### Shopping Cart
- Session-based cart management
- Quantity updates with stock validation
- Real-time price calculations
- Persistent cart across sessions

### Order Management
- Complete checkout process
- Order confirmation with details
- Admin order status updates
- Order history tracking

### Admin Features
- Product CRUD operations
- User management
- Order status management
- Dashboard with quick actions

### Security
- Role-based authorization (Admin/Customer)
- Password requirements
- CSRF protection
- Input validation

## Database Relationships
- Products → Categories (Many-to-One)
- CartDetails → Carts (Many-to-One)
- CartDetails → Products (Many-to-One)
- Orders → Users (Many-to-One)
- OrderDetails → Orders (Many-to-One)
- Reviews → Products & Users (Many-to-One)

## Future Enhancements
- Payment gateway integration
- Email notifications
- Product image upload
- Advanced reporting
- Inventory alerts
- Customer reviews system
- Wishlist functionality

## License
This project is for educational purposes.