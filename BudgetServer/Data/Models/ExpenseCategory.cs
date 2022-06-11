namespace BudgetServer.Data.Models;

public class ExpenseCategory
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int ExpenseCategoryGroupId { get; set; }
    public ExpenseCategoryGroup ExpenseCategoryGroup { get; set; }

}
