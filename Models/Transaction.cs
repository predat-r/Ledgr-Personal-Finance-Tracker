using System.ComponentModel.DataAnnotations;

namespace Ledgr.Models;

public class Transaction
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [Required]
    public TransactionType Type { get; set; }
    
    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    [Required]
    public int CategoryId { get; set; }
    
    public Category? Category { get; set; }
    
    public string? UserId { get; set; }
    
    public ApplicationUser? User { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsRecurring { get; set; }
    
    public DateTime? RecurringUntil { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
}