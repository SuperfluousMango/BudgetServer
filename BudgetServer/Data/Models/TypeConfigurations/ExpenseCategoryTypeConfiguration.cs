using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetServer.Data.Models.TypeConfigurations;

public class ExpenseCategoryTypeConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.HasOne(x => x.ExpenseCategoryGroup)
            .WithMany(x => x.ExpenseCategories);

        builder.HasIndex(x => new { x.Name, x.ExpenseCategoryGroupId })
            .IsUnique();

        builder.HasData(
            new ExpenseCategory { Id = 1, ExpenseCategoryGroupId = 1, Name = "Mortgage" },
            new ExpenseCategory { Id = 2, ExpenseCategoryGroupId = 1, Name = "Water bill" },
            new ExpenseCategory { Id = 3, ExpenseCategoryGroupId = 1, Name = "Electric bill" },
            new ExpenseCategory { Id = 4, ExpenseCategoryGroupId = 1, Name = "Gas bill" }
        );
    }
}
