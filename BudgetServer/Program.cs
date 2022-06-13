using BudgetServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app);
ConfigureEndpoints(app);

app.Run();

void ConfigureServices(IServiceCollection services, ConfigurationManager configuration) {
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget API", Version = "v1" });
    });

    services.AddDbContext<BudgetContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("BudgetDb"));
    });

    services.AddCors(setup =>
    {
        setup.AddDefaultPolicy(policy =>
        {
            // Load allowed origins from appsettings and decorate them with all the schemes
            var allowedOrigins = (configuration.GetSection("CorsOrigins").Get<string[]>() ?? Array.Empty<string>())
                .SelectMany(x => new string[] { x, $"http://{x}", $"https://{x}" })
                .ToArray();
            policy.WithMethods("GET", "POST", "PUT", "DELETE");
            policy.WithOrigins(allowedOrigins);
            policy.WithHeaders("Content-Type");
        });
    });
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Budget API V1");
        });
    }

    app.UseHttpsRedirection();
}

void ConfigureEndpoints(WebApplication app)
{
    app.UseRouting();

    app.UseCors();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
