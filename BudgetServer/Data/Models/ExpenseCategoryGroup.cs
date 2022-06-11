namespace BudgetServer.Data.Models;

public class ExpenseCategoryGroup
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();
}
