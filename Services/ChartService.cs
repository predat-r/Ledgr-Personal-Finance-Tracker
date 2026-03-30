using Microsoft.EntityFrameworkCore;
using Ledgr.Data;
using Ledgr.Models;

namespace Ledgr.Services;

public class ChartData
{
    public List<string> Labels { get; set; } = new();
    public List<decimal> Data { get; set; } = new();
    public List<string> Colors { get; set; } = new();
}

public interface IChartService
{
    Task<ChartData> GetExpensesByCategoryAsync(string userId, DateTime? start = null, DateTime? end = null);
    Task<ChartData> GetIncomeByCategoryAsync(string userId, DateTime? start = null, DateTime? end = null);
    Task<List<(DateTime Date, decimal Amount)>> GetDailyBalanceAsync(string userId, DateTime start, DateTime end);
    Task<List<(string Month, decimal Income, decimal Expenses)>> GetMonthlyTrendsAsync(string userId, int months = 6);
}

public class ChartService : IChartService
{
    private readonly LedgrDbContext _context;

    public ChartService(LedgrDbContext context)
    {
        _context = context;
    }

    public async Task<ChartData> GetExpensesByCategoryAsync(string userId, DateTime? start = null, DateTime? end = null)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense);

        if (start.HasValue)
            query = query.Where(t => t.Date >= start.Value);
        if (end.HasValue)
            query = query.Where(t => t.Date <= end.Value);

        var result = await query
            .GroupBy(t => t.Category!.Name)
            .Select(g => new { Name = g.Key, Total = g.Sum(t => t.Amount), Color = g.First().Category!.Color })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        return new ChartData
        {
            Labels = result.Select(r => r.Name).ToList(),
            Data = result.Select(r => r.Total).ToList(),
            Colors = result.Select(r => r.Color ?? "#6c757d").ToList()
        };
    }

    public async Task<ChartData> GetIncomeByCategoryAsync(string userId, DateTime? start = null, DateTime? end = null)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.Type == TransactionType.Income);

        if (start.HasValue)
            query = query.Where(t => t.Date >= start.Value);
        if (end.HasValue)
            query = query.Where(t => t.Date <= end.Value);

        var result = await query
            .GroupBy(t => t.Category!.Name)
            .Select(g => new { Name = g.Key, Total = g.Sum(t => t.Amount), Color = g.First().Category!.Color })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        return new ChartData
        {
            Labels = result.Select(r => r.Name).ToList(),
            Data = result.Select(r => r.Total).ToList(),
            Colors = result.Select(r => r.Color ?? "#28a745").ToList()
        };
    }

    public async Task<List<(DateTime Date, decimal Amount)>> GetDailyBalanceAsync(string userId, DateTime start, DateTime end)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && t.Date >= start && t.Date <= end)
            .ToListAsync();

        var dailyBalances = transactions
            .GroupBy(t => t.Date.Date)
            .Select(g => (
                Date: g.Key,
                Amount: g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) -
                        g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            ))
            .OrderBy(x => x.Date)
            .ToList();

        return dailyBalances;
    }

    public async Task<List<(string Month, decimal Income, decimal Expenses)>> GetMonthlyTrendsAsync(string userId, int months = 6)
    {
        var startDate = DateTime.UtcNow.AddMonths(-months + 1).Date;
        startDate = new DateTime(startDate.Year, startDate.Month, 1);

        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && t.Date >= startDate)
            .ToListAsync();

        var result = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => (
                Month: new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                Income: g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expenses: g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            ))
            .OrderBy(x => x.Month)
            .ToList();

        return result;
    }
}