using BudgetServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetServer.Data.TypeConfigurations;

public class ExpenseCategoryTypeConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.HasData(
            new ExpenseCategory { Id = 1, ExpenseCategoryGroupId = 1, Name = "Mortgage" },
            new ExpenseCategory { Id = 2, ExpenseCategoryGroupId = 1, Name = "Water bill" },
            new ExpenseCategory { Id = 3, ExpenseCategoryGroupId = 1, Name = "Electric bill" },
            new ExpenseCategory { Id = 4, ExpenseCategoryGroupId = 1, Name = "Gas bill" }
        );

        builder.HasOne(x => x.ExpenseCategoryGroup)
            .WithMany(x => x.ExpenseCategories);
    }
}
