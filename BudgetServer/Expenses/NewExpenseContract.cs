namespace BudgetServer.Expenses;

public class NewExpenseContract
{
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
    public DateTime TransactionDate { get; set; }
    public int CategoryId { get; set; }
}
