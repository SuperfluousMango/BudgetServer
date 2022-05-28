namespace BudgetServer.Data.Models;

public class TransactionCategoryGroup
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<TransactionCategory> TransactionCategories { get; set; } = new List<TransactionCategory>();
}
