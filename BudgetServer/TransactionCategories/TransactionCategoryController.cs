using BudgetServer.Data;
using BudgetServer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BudgetServer.TransactionCategories;
[Route("api/[controller]")]
[ApiController]
public class TransactionCategoryController : ControllerBase
{
    private readonly BudgetContext _budgetContext;

    public TransactionCategoryController(BudgetContext budgetContext)
    {
        _budgetContext = budgetContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken token)
    {
        var groupedCategories = await _budgetContext.TransactionCategories
            .Include(c => c.TransactionCategoryGroup)
            .GroupBy(c => c.TransactionCategoryGroup.Name)
            .Select(g => new {
                Name = g.Key,
                Categories = g.Select(
                    c => new TransactionCategoryDisplayContract { Id = c.Id, Name = c.Name, DisplayName = $"{g.Key} – {c.Name}" }
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

        var group = await _budgetContext.TransactionCategoryGroups
            .Include(x => x.TransactionCategories)
            .FirstOrDefaultAsync(x => x.Name.ToLower() == contract.GroupName.ToLower(), token);

        if (group != null && group.TransactionCategories.Any(x => x.Name.ToLower() == contract.CategoryName.ToLower()))
        {
            return BadRequest();
        }

        if (group == null)
        {
            group = new TransactionCategoryGroup { Name = GetTitleCase(contract.GroupName) };
            _budgetContext.TransactionCategoryGroups.Add(group);
        }

        var category = new TransactionCategory
        {
            Name = GetTitleCase(contract.CategoryName),
            TransactionCategoryGroup = group
        };
        _budgetContext.TransactionCategories.Add(category);
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

    private class TransactionCategoryDisplayContract
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
