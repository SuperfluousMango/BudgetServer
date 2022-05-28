namespace BudgetServer.Data.Models;

public class TransactionEntry
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

    public int TransactionCategoryId { get; set; }
    public TransactionCategory TransactionCategory { get; set; }
}
