using BudgetServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetServer.Data.TypeConfigurations;

public class ExpenseTypeConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.Property(x => x.Amount)
            .HasColumnType("decimal(7,2)");

        builder.Property(x => x.Memo)
            .HasMaxLength(100);

        builder.Property(x => x.TransactionDate)
            .HasColumnType("DATE");

        builder.Property(x => x.CreatedDate)
            .HasPrecision(0);

        builder.HasOne(x => x.ExpenseCategory);
    }
}
