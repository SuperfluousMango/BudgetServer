using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetServer.Data.Models.TypeConfigurations;

public class ExpenseCategoryGroupTypeConfiguration : IEntityTypeConfiguration<ExpenseCategoryGroup>
{
    public void Configure(EntityTypeBuilder<ExpenseCategoryGroup> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasData(
            new ExpenseCategoryGroup { Id = 1, Name = "Home" }
        );
    }
}
