using BudgetServer.Data;
using BudgetServer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetServer.Expenses;
[Route("api/[controller]")]
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
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        return _dbContext.Expenses.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
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
    public async Task<IActionResult> Post([FromBody] ExpenseContract contract, CancellationToken token)
    {
        if (contract == null || contract?.Id != 0 || !ExpenseIsValid(contract))
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put([FromBody] ExpenseContract contract, CancellationToken token)
    {
        if (contract == null || contract?.Id == 0 || !ExpenseIsValid(contract))
        {
            return BadRequest();
        }

        var expense = await _dbContext.Expenses.FirstOrDefaultAsync(x => x.Id == contract!.Id, token);
        if (expense == null)
        {
            return NotFound();
        }

        expense.Amount = contract!.Amount;
        expense.ExpenseCategoryId = contract.CategoryId;
        expense.Memo = contract.Memo;
        expense.TransactionDate = contract.TransactionDate;
        await _dbContext.SaveChangesAsync(token);

        return Ok();
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
                    Id = x.Id,
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

    private static bool ExpenseIsValid(ExpenseContract? contract)
    {
        return contract.TransactionDate != default &&
            contract.Amount > 0 &&
            contract.CategoryId > 0;
    }

    public class ExpenseInfo
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; } = "";
    }

    public class ExpenseGroup
    {
        public string Name { get; set; } = "";
        public decimal Total { get; set; }
    }
}
