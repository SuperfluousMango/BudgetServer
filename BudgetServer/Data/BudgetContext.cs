using BudgetServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BudgetServer.Data;

public class BudgetContext : DbContext
{
    public BudgetContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<TransactionEntry> TransactionEntries => Set<TransactionEntry>();
    public DbSet<TransactionCategory> TransactionCategories => Set<TransactionCategory>();
    public DbSet<TransactionCategoryGroup> TransactionCategoryGroups => Set<TransactionCategoryGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BudgetContext).Assembly);

        //This will singularize all table names
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
        }
    }
}
