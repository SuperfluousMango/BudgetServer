namespace BudgetServer.Data.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
    public bool IsDeleted { get; set; }

    public int ExpenseCategoryId { get; set; }
    public ExpenseCategory ExpenseCategory { get; set; }
}
