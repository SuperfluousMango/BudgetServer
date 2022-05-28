namespace BudgetServer.Data.Models;

public class TransactionCategory
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int TransactionCategoryGroupId { get; set; }
    public TransactionCategoryGroup TransactionCategoryGroup { get; set; }

}
