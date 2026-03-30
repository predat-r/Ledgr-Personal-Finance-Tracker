# Ledgr - Personal Finance Tracker

A Blazor Server application for tracking personal finances, managing transactions, categorizing spending, and visualizing financial data with interactive charts.

## Features

### 🔐 Authentication
- User registration and login using ASP.NET Identity
- Secure database storage for user-specific data

### 💰 Transaction Management
- Add, edit, and delete income/expense transactions
- Categorize transactions (pre-seeded + custom categories)
- Filter transactions by date, type, and category
- Pagination for large datasets

### 📊 Dashboard
- Real-time balance overview
- Monthly income and expense summary
- Pie chart showing expense breakdown by category
- Recent transactions list

### 📈 Reports & Analytics
- Detailed financial reports with date range filtering
- Expense breakdown by category (pie chart)
- Income breakdown by category (pie chart)
- Monthly trends visualization (bar chart)
- Savings rate calculation

### 🎨 Categories
- Pre-seeded income categories: Salary, Freelance, Investments, Other Income
- Pre-seeded expense categories: Food & Dining, Transportation, Utilities, Entertainment, Shopping, Healthcare, Rent, Other Expense
- Custom category creation with color selection

## Technology Stack

- **Framework**: .NET 8.0 / Blazor Server
- **Database**: SQLite (via Entity Framework Core)
- **Authentication**: ASP.NET Identity
- **UI Components**: Blazorise with Bootstrap
- **Charts**: Chart.js via ChartJs.Blazor

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- A code editor (VS Code, Visual Studio, or Rider)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/predat-r/Ledgr-Personal-Finance-Tracker.git
cd Ledgr
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

4. Open your browser and navigate to:
```
https://localhost:5001
```

### First Run
- On first run, the database will be automatically created
- Register a new account to start tracking your finances
- Default categories will be seeded automatically

## Project Structure

```
Ledgr/
├── Data/
│   └── LedgrDbContext.cs       # Entity Framework DbContext
├── Models/
│   ├── ApplicationUser.cs      # Custom identity user
│   ├── Category.cs             # Transaction categories
│   └── Transaction.cs          # Income/Expense transactions
├── Services/
│   ├── CategoryService.cs      # Category CRUD operations
│   ├── ChartService.cs         # Chart data preparation
│   ├── DashboardService.cs     # Dashboard summary data
│   ├── TransactionService.cs   # Transaction CRUD operations
│   └── UserService.cs          # User management
├── Pages/
│   ├── Index.razor             # Dashboard
│   ├── Transactions.razor       # Transaction list
│   ├── AddTransaction.razor     # Add new transaction
│   ├── EditTransaction.razor   # Edit transaction
│   ├── Categories.razor        # Category management
│   └── Reports.razor           # Financial reports
├── Layout/
│   ├── MainLayout.razor        # Main application layout
│   └── NavMenu.razor           # Navigation sidebar
└── wwwroot/
    └── css/
        └── app.css             # Custom styles
```

## Configuration

### Database
The application uses SQLite by default. The connection string is in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ledgr.db"
  }
}
```

### Changing the Database
To use a different database (e.g., SQL Server), update the connection string and install the appropriate NuGet package:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Then update `Program.cs`:
```csharp
options.UseSqlServer(connectionString);
```

## Screenshots

### Dashboard
- Balance overview with income/expense cards
- Expense breakdown pie chart
- Top spending categories
- Recent transactions table

### Transactions
- Searchable and filterable transaction list
- Add/Edit/Delete functionality
- Pagination support

### Reports
- Date range selector
- Income vs Expenses summary
- Savings rate calculation
- Category breakdown with percentages
- Monthly trends visualization

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.