using BudgetServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetServer.Data.TypeConfigurations;

public class TransactionCategoryTypeConfiguration : IEntityTypeConfiguration<TransactionCategory>
{
    public void Configure(EntityTypeBuilder<TransactionCategory> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.HasData(
            new TransactionCategory { Id = 1, TransactionCategoryGroupId = 1, Name = "Mortgage" },
            new TransactionCategory { Id = 2, TransactionCategoryGroupId = 1, Name = "Water bill" },
            new TransactionCategory { Id = 3, TransactionCategoryGroupId = 1, Name = "Electric bill" },
            new TransactionCategory { Id = 4, TransactionCategoryGroupId = 1, Name = "Gas bill" }
        );

        builder.HasOne(x => x.TransactionCategoryGroup)
            .WithMany(x => x.TransactionCategories);
    }
}
