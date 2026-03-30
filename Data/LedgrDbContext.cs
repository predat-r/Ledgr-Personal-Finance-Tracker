using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ledgr.Models;

namespace Ledgr.Data;

public class LedgrDbContext : IdentityDbContext<ApplicationUser>
{
    public LedgrDbContext(DbContextOptions<LedgrDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Transaction>(entity =>
        {
            entity.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            entity.HasIndex(t => t.Date);
            entity.HasIndex(t => t.UserId);
        });

        builder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();
        });

        // Seed default categories
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Salary", Type = CategoryType.Income, Color = "#28a745", Icon = "fa-money-bill" },
            new Category { Id = 2, Name = "Freelance", Type = CategoryType.Income, Color = "#17a2b8", Icon = "fa-laptop" },
            new Category { Id = 3, Name = "Investments", Type = CategoryType.Income, Color = "#ffc107", Icon = "fa-chart-line" },
            new Category { Id = 4, Name = "Other Income", Type = CategoryType.Income, Color = "#6c757d", Icon = "fa-plus-circle" },
            new Category { Id = 5, Name = "Food & Dining", Type = CategoryType.Expense, Color = "#dc3545", Icon = "fa-utensils" },
            new Category { Id = 6, Name = "Transportation", Type = CategoryType.Expense, Color = "#fd7e14", Icon = "fa-car" },
            new Category { Id = 7, Name = "Utilities", Type = CategoryType.Expense, Color = "#20c997", Icon = "fa-bolt" },
            new Category { Id = 8, Name = "Entertainment", Type = CategoryType.Expense, Color = "#e83e8c", Icon = "fa-film" },
            new Category { Id = 9, Name = "Shopping", Type = CategoryType.Expense, Color = "#6f42c1", Icon = "fa-shopping-bag" },
            new Category { Id = 10, Name = "Healthcare", Type = CategoryType.Expense, Color = "#007bff", Icon = "fa-heartbeat" },
            new Category { Id = 11, Name = "Rent", Type = CategoryType.Expense, Color = "#343a40", Icon = "fa-home" },
            new Category { Id = 12, Name = "Other Expense", Type = CategoryType.Expense, Color = "#6c757d", Icon = "fa-minus-circle" }
        );
    }
}