using Microsoft.EntityFrameworkCore;
using Ledgr.Data;
using Ledgr.Models;

namespace Ledgr.Services;

public interface ITransactionService
{
    Task<List<Transaction>> GetUserTransactionsAsync(string userId);
    Task<List<Transaction>> GetUserTransactionsByDateRangeAsync(string userId, DateTime start, DateTime end);
    Task<Transaction?> GetTransactionByIdAsync(int id, string userId);
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction> UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(int id, string userId);
    Task<decimal> GetTotalIncomeAsync(string userId, DateTime? start = null, DateTime? end = null);
    Task<decimal> GetTotalExpensesAsync(string userId, DateTime? start = null, DateTime? end = null);
    Task<decimal> GetCurrentBalanceAsync(string userId);
    Task<List<Transaction>> GetRecentTransactionsAsync(string userId, int count = 5);
}

public class TransactionService : ITransactionService
{
    private readonly LedgrDbContext _context;

    public TransactionService(LedgrDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetUserTransactionsAsync(string userId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetUserTransactionsByDateRangeAsync(string userId, DateTime start, DateTime end)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.Date >= start && t.Date <= end)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id, string userId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task DeleteTransactionAsync(int id, string userId)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalIncomeAsync(string userId, DateTime? start = null, DateTime? end = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Income);

        if (start.HasValue)
            query = query.Where(t => t.Date >= start.Value);
        if (end.HasValue)
            query = query.Where(t => t.Date <= end.Value);

        return await query.SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetTotalExpensesAsync(string userId, DateTime? start = null, DateTime? end = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense);

        if (start.HasValue)
            query = query.Where(t => t.Date >= start.Value);
        if (end.HasValue)
            query = query.Where(t => t.Date <= end.Value);

        return await query.SumAsync(t => t.Amount);
    }

    public async Task<decimal> GetCurrentBalanceAsync(string userId)
    {
        var income = await GetTotalIncomeAsync(userId);
        var expenses = await GetTotalExpensesAsync(userId);
        return income - expenses;
    }

    public async Task<List<Transaction>> GetRecentTransactionsAsync(string userId, int count = 5)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .Take(count)
            .ToListAsync();
    }
}