using Microsoft.EntityFrameworkCore;
using Ledgr.Data;
using Ledgr.Models;

namespace Ledgr.Services;

public class DashboardSummary
{
    public decimal TotalBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal AverageDailyExpense { get; set; }
    public int TransactionCount { get; set; }
    public decimal LargestExpense { get; set; }
    public string? LargestExpenseCategory { get; set; }
    public List<TopCategorySpending> TopExpenseCategories { get; set; } = new();
    public List<Transaction> RecentTransactions { get; set; } = new();
}

public class TopCategorySpending
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public string? Color { get; set; }
}

public class DashboardService
{
    private readonly LedgrDbContext _context;
    private readonly ITransactionService _transactionService;
    private readonly IChartService _chartService;

    public DashboardService(LedgrDbContext context, ITransactionService transactionService, IChartService chartService)
    {
        _context = context;
        _transactionService = transactionService;
        _chartService = chartService;
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync(string userId)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        var daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);

        var balance = await _transactionService.GetCurrentBalanceAsync(userId);
        var totalIncome = await _transactionService.GetTotalIncomeAsync(userId, startOfMonth, endOfMonth);
        var totalExpenses = await _transactionService.GetTotalExpensesAsync(userId, startOfMonth, endOfMonth);
        var recentTransactions = await _transactionService.GetRecentTransactionsAsync(userId, 5);

        var averageDailyExpense = daysInMonth > 0 ? totalExpenses / daysInMonth : 0;

        var topCategories = await _chartService.GetExpensesByCategoryAsync(userId, startOfMonth, endOfMonth);
        var totalExpenseAmount = topCategories.Data.Sum();

        var topSpending = topCategories.Labels
            .Take(5)
            .Select((label, index) => new TopCategorySpending
            {
                CategoryName = label,
                Amount = index < topCategories.Data.Count ? topCategories.Data[index] : 0,
                Percentage = totalExpenseAmount > 0 && index < topCategories.Data.Count 
                    ? (topCategories.Data[index] / totalExpenseAmount) * 100 : 0,
                Color = index < topCategories.Colors.Count ? topCategories.Colors[index] : "#6c757d"
            })
            .ToList();

        var largestExpense = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense)
            .OrderByDescending(t => t.Amount)
            .FirstOrDefaultAsync();

        return new DashboardSummary
        {
            TotalBalance = balance,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            AverageDailyExpense = averageDailyExpense,
            TransactionCount = await _context.Transactions.CountAsync(t => t.UserId == userId),
            LargestExpense = largestExpense?.Amount ?? 0,
            LargestExpenseCategory = largestExpense?.Category?.Name,
            TopExpenseCategories = topSpending,
            RecentTransactions = recentTransactions
        };
    }
}