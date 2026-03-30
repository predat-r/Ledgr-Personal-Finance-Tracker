using System.ComponentModel.DataAnnotations;

namespace Ledgr.Models;

public class Category
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public CategoryType Type { get; set; }
    
    public string? Color { get; set; }
    
    public string? Icon { get; set; }
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum CategoryType
{
    Income,
    Expense
}