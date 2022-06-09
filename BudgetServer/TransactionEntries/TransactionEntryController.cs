using BudgetServer.Data;
using BudgetServer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetServer.TransactionEntries;
[Route("api/[controller]")]
[ApiController]
public class TransactionEntryController : ControllerBase
{
    private BudgetContext _dbContext;

    public TransactionEntryController(BudgetContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{year:int}/{month:int}")]
    public Task<List<TransactionEntry>> Get(int year, int month, CancellationToken token)
    {
        var startDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = startDate.AddMonths(1);

        return _dbContext.TransactionEntries.Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(token);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        var transactionEntry = await _dbContext.TransactionEntries
            .FirstOrDefaultAsync(t => t.Id == id, token);
        
        return transactionEntry == null
            ? NotFound()
            : Ok(transactionEntry);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] NewTransactionEntryContract contract, CancellationToken token)
    {
        if (!TransactionIsValid(contract))
        {
            return BadRequest();
        }

        var transactionEntry = new TransactionEntry
        {
            Amount = contract.Amount,
            TransactionCategoryId = contract.CategoryId,
            Memo = contract.Memo,
            TransactionDate = contract.TransactionDate.Date,
            CreatedDate = DateTimeOffset.Now
        };
        _dbContext.TransactionEntries.Add(transactionEntry);
        await _dbContext.SaveChangesAsync(token);

        var url = Url.ActionLink(action: "Get", values: new { transactionEntry.Id }) ?? "";

        return Created(url, transactionEntry);
    }

    [HttpGet("Recent")]
    public Task<List<TransactionInfo>> GetRecentTransactions(CancellationToken token)
    {
        return _dbContext.TransactionEntries
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.CreatedDate)
            .Take(5)
            .Select(x => new TransactionInfo()
                {
                    TransactionDate = x.TransactionDate,
                    Amount = x.Amount,
                    Memo = x.Memo ?? $"{x.TransactionCategory.TransactionCategoryGroup.Name} – {x.TransactionCategory.Name}"
                })
            .ToListAsync(token);
    }

    [HttpGet("RecentGrouped/{year:int}/{month:int}")]
    public Task<List<TransactionGroup>> GetRecentGroupedTransactions(int year, int month, CancellationToken token)
    {
        var startDate = new DateTimeOffset(year, month, 1, 0, 0, 0, DateTimeOffset.Now.Offset);
        var endDate = startDate.AddMonths(1);

        return _dbContext.TransactionEntries
            .Where(tr => tr.TransactionDate >= startDate && tr.TransactionDate < endDate)
            .GroupBy(x => x.TransactionCategory.TransactionCategoryGroup.Name)
            .Select(x => new TransactionGroup { Name = x.Key, Total = x.Sum(tr => tr.Amount) })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Name)
            .ToListAsync(token);
    }

    private static bool TransactionIsValid(NewTransactionEntryContract contract)
    {
        return contract != null &&
            contract.TransactionDate != default &&
            contract.Amount > 0 &&
            contract.CategoryId > 0;
    }

    public class TransactionInfo
    {
        public DateTimeOffset TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; } = "";
    }

    public class TransactionGroup
    {
        public string Name { get; set; } = "";
        public decimal Total { get; set; }
    }
}
