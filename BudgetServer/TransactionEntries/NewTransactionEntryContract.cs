namespace BudgetServer.TransactionEntries;

public class NewTransactionEntryContract
{
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
    public int CategoryId { get; set; }
}
