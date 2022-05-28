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
    public async Task<IActionResult> Get(int year, int month, CancellationToken token)
    {
        var startDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = startDate.AddMonths(1);

        var list = await _dbContext.TransactionEntries.Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(token);

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        var transactionEntry = await _dbContext.TransactionEntries
            .FirstOrDefaultAsync(t => t.Id == id);
        
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
            TransactionDate = contract.TransactionDate,
            CreatedDate = DateTimeOffset.Now
        };
        _dbContext.TransactionEntries.Add(transactionEntry);
        await _dbContext.SaveChangesAsync(token);

        var url = Url.ActionLink(action: "Get", values: new { transactionEntry.Id }) ?? "";

        return Created(url, transactionEntry);
    }

    private bool TransactionIsValid(NewTransactionEntryContract contract)
    {
        return contract != null &&
            contract.TransactionDate != default &&
            contract.Amount > 0 &&
            contract.CategoryId > 0;
    }
}
