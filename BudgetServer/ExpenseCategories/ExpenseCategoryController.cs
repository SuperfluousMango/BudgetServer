using BudgetServer.Data;
using BudgetServer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BudgetServer.ExpenseCategories;
[Route("api/[controller]")]
[ApiController]
public class ExpenseCategoryController : ControllerBase
{
    private const char EN_DASH = '–';
    
    private readonly BudgetContext _budgetContext;

    public ExpenseCategoryController(BudgetContext budgetContext)
    {
        _budgetContext = budgetContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken token)
    {
        var groupedCategories = await _budgetContext.ExpenseCategories
            .Include(c => c.ExpenseCategoryGroup)
            .GroupBy(c => c.ExpenseCategoryGroup.Name)
            .Select(g => new
            {
                Name = g.Key,
                Categories = g.Select(
                    c => new ExpenseCategoryDisplayContract { Id = c.Id, Name = c.Name, DisplayName = g.Key == c.Name ? g.Key : $"{g.Key} {EN_DASH} {c.Name}" }
                )
                .OrderBy(c => c.Name)
                .ToList()
            })
            .OrderBy(g => g.Name)
            .ToListAsync(token);

        return Ok(groupedCategories);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(NewCategoryContract contract, CancellationToken token)
    {
        if (contract == null || string.IsNullOrWhiteSpace(contract.CategoryName) || string.IsNullOrWhiteSpace(contract.GroupName))
        {
            return BadRequest();
        }

        var group = await _budgetContext.ExpenseCategoryGroups
            .Include(x => x.ExpenseCategories)
            .FirstOrDefaultAsync(x => x.Name.ToLower() == contract.GroupName.ToLower(), token);

        if (group != null && group.ExpenseCategories.Any(x => x.Name.ToLower() == contract.CategoryName.ToLower()))
        {
            return BadRequest();
        }

        if (group == null)
        {
            group = new ExpenseCategoryGroup { Name = GetTitleCase(contract.GroupName) };
            _budgetContext.ExpenseCategoryGroups.Add(group);
        }

        var category = new ExpenseCategory
        {
            Name = GetTitleCase(contract.CategoryName),
            ExpenseCategoryGroup = group
        };
        _budgetContext.ExpenseCategories.Add(category);
        await _budgetContext.SaveChangesAsync(token);

        return NoContent();
    }

    private string GetTitleCase(string name)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name)
            .Replace(" The ", " the ")
            .Replace(" An ", " an ")
            .Replace(" A ", " a ")
            .Replace(" And ", " and ")
            .Replace(" Or ", " or ");
    }

    private class ExpenseCategoryDisplayContract
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class NewCategoryContract
    {
        public string CategoryName { get; set; }
        public string GroupName { get; set; }
    }
}
