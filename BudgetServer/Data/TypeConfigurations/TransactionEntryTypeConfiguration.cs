using BudgetServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetServer.Data.TypeConfigurations;

public class TransactionEntryTypeConfiguration : IEntityTypeConfiguration<TransactionEntry>
{
    public void Configure(EntityTypeBuilder<TransactionEntry> builder)
    {
        builder.Property(x => x.Amount)
            .HasColumnType("decimal(7,2)");

        builder.Property(x => x.Memo)
            .HasMaxLength(100);

        builder.HasOne(x => x.TransactionCategory);
    }
}
