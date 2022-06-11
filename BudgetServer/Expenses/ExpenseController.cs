using BudgetServer.Data;
using BudgetServer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetServer.Expenses;
[Route("api/[controller]")]
[Route("api/TransactionEntry")] // Maintain compatibility with old client expectations until it's updated
[ApiController]
public class ExpenseController : ControllerBase
{
    private BudgetContext _dbContext;

    public ExpenseController(BudgetContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{year:int}/{month:int}")]
    public Task<List<Expense>> Get(int year, int month, CancellationToken token)
    {
        var startDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = startDate.AddMonths(1);

        return _dbContext.Expenses.Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(token);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        var expense = await _dbContext.Expenses
            .FirstOrDefaultAsync(t => t.Id == id, token);
        
        return expense == null
            ? NotFound()
            : Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] NewExpenseContract contract, CancellationToken token)
    {
        if (!ExpenseIsValid(contract))
        {
            return BadRequest();
        }

        var expense = new Expense
        {
            Amount = contract.Amount,
            ExpenseCategoryId = contract.CategoryId,
            Memo = contract.Memo,
            TransactionDate = contract.TransactionDate.Date,
            CreatedDate = DateTimeOffset.Now
        };
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync(token);

        var url = Url.ActionLink(action: "Get", values: new { expense.Id }) ?? "";

        return Created(url, expense);
    }

    [HttpGet("Recent")]
    public Task<List<ExpenseInfo>> GetRecentExpenses(CancellationToken token)
    {
        return _dbContext.Expenses
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.CreatedDate)
            .Take(5)
            .Select(x => new ExpenseInfo()
                {
                    TransactionDate = x.TransactionDate,
                    Amount = x.Amount,
                    Memo = x.Memo ?? $"{x.ExpenseCategory.ExpenseCategoryGroup.Name} – {x.ExpenseCategory.Name}"
                })
            .ToListAsync(token);
    }

    [HttpGet("RecentGrouped/{year:int}/{month:int}")]
    public Task<List<ExpenseGroup>> GetRecentGroupedExpenses(int year, int month, CancellationToken token)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        return _dbContext.Expenses
            .Where(tr => tr.TransactionDate >= startDate && tr.TransactionDate < endDate)
            .GroupBy(x => x.ExpenseCategory.ExpenseCategoryGroup.Name)
            .Select(x => new ExpenseGroup { Name = x.Key, Total = x.Sum(tr => tr.Amount) })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Name)
            .ToListAsync(token);
    }

    private static bool ExpenseIsValid(NewExpenseContract contract)
    {
        return contract != null &&
            contract.TransactionDate != default &&
            contract.Amount > 0 &&
            contract.CategoryId > 0;
    }

    public class ExpenseInfo
    {
        public DateTimeOffset TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; } = "";
    }

    public class ExpenseGroup
    {
        public string Name { get; set; } = "";
        public decimal Total { get; set; }
    }
}
