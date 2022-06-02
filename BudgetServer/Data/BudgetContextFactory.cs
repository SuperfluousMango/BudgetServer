using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics;
using System.Linq;

namespace BudgetServer.Data;

public class BudgetContextFactory : IDesignTimeDbContextFactory<BudgetContext>
{
    public BudgetContext CreateDbContext(string[] args)
    {
        string path = Directory.GetCurrentDirectory();

        IConfigurationBuilder builder =
            new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json");

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configFile = env == "Production"
            ? "appsettings.Production.json"
            : "appsettings.Development.json";
        builder.AddJsonFile(configFile);

        IConfigurationRoot config = builder.Build();

        string connectionString = config.GetConnectionString("BudgetDbMigrations");

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BudgetContext>();
        dbContextOptionsBuilder.UseSqlServer(connectionString);

        return new BudgetContext(dbContextOptionsBuilder.Options);
    }
}
