using BudgetServer.Data;
using BudgetServer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetServer.Expenses;
[Route("api/[controller]")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private const char EN_DASH = '–';

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

        return _dbContext.Expenses.Where(e => e.TransactionDate >= startDate && e.TransactionDate <= endDate)
            .OrderByDescending(e => e.TransactionDate)
            .ToListAsync(token);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken token)
    {
        var expense = await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == id, token);
        
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
        if (contract == null || contract?.Id <= 0 || !ExpenseIsValid(contract!))
        {
            return BadRequest();
        }

        var expense = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == contract!.Id, token);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var expense = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id, token);
        if (expense == null)
        {
            return NotFound();
        }

        _dbContext.Expenses.Remove(expense);
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
            .Select(e => new ExpenseInfo()
                {
                    Id = e.Id,
                    TransactionDate = e.TransactionDate,
                    Amount = e.Amount,
                    Memo = e.Memo ??
                        (e.ExpenseCategory.ExpenseCategoryGroup.Name == e.ExpenseCategory.Name
                            ? e.ExpenseCategory.Name
                            : $"{e.ExpenseCategory.ExpenseCategoryGroup.Name} {EN_DASH} {e.ExpenseCategory.Name}")
            })
            .ToListAsync(token);
    }

    [HttpGet("RecentByGroup/{year:int}/{month:int}")]
    [HttpGet("RecentGrouped/{year:int}/{month:int}")] // Backwards compatibility
    public Task<List<ExpensesByCategoryGroup>> GetRecentGroupedExpenses(int year, int month, CancellationToken token)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        return _dbContext.Expenses
            .Where(e => e.TransactionDate >= startDate && e.TransactionDate < endDate)
            .GroupBy(e => new { Id = e.ExpenseCategory.ExpenseCategoryGroupId, Name = e.ExpenseCategory.ExpenseCategoryGroup.Name })
            .Select(g => new ExpensesByCategoryGroup { Id = g.Key.Id, Name = g.Key.Name, Total = g.Sum(tr => tr.Amount) })
            .OrderByDescending(g => g.Total)
            .ThenBy(g => g.Name)
            .ToListAsync(token);
    }

    [HttpGet("RecentByCategory/{year:int}/{month:int}/{categoryGroupId:int}")]
    public Task<List<ExpensesByCategory>> GetRecentExpensesByCategoryGroup(int year, int month, int categoryGroupId, CancellationToken token)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        return _dbContext.Expenses
            .Where(e => e.TransactionDate >= startDate && e.TransactionDate < endDate && e.ExpenseCategory.ExpenseCategoryGroupId == categoryGroupId)
            .GroupBy(e => new { Id = e.ExpenseCategoryId, Name = e.ExpenseCategory.Name })
            .Select(g => new ExpensesByCategory { Id = g.Key.Id, Name = g.Key.Name, Total = g.Sum(e => e.Amount) })
            .OrderByDescending(g => g.Total)
            .ThenBy(g => g.Name)
            .ToListAsync(token);
    }

    private static bool ExpenseIsValid(ExpenseContract contract)
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

    public class ExpensesByCategoryGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Total { get; set; }
    }

    public class ExpensesByCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Total { get; set; }
    }
}
