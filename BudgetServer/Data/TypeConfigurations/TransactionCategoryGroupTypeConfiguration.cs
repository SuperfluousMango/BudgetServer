using BudgetServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetServer.Data.TypeConfigurations;

public class TransactionCategoryGroupTypeConfiguration : IEntityTypeConfiguration<TransactionCategoryGroup>
{
    public void Configure(EntityTypeBuilder<TransactionCategoryGroup> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.HasData(
            new TransactionCategoryGroup { Id = 1, Name = "Home" }
        );
    }
}
