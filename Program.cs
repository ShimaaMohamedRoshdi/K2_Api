using AccountDocApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure PostgreSQL DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5432;Database=AccountDocDb;Username=postgres;Password=12345;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure CORS for Nintex / Web clients
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Swagger / OpenAPI for Nintex Workflow integration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Account & Document Integration API (Nintex Compatible)",
        Version = "v1",
        Description = "REST API for Nintex Workflows to query Customer Accounts and Document Signatures with Base64 encoded file contents."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Remove 'additionalProperties: false' constraint for K2 compatibility
    c.SchemaFilter<RemoveAdditionalPropertiesSchemaFilter>();

    // Filter out non-200 responses so K2 REST Service Broker expands individual DTO properties instead of fallback 'Memo'
    c.OperationFilter<K2ResponseFilter>();
});

var app = builder.Build();

// Initialize Supabase PostgreSQL database tables and seed data
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        logger.LogInformation("Initializing PostgreSQL database tables and seed data...");
        await DbInitializer.InitializeAsync(dbContext);
        logger.LogInformation("Supabase database tables created and seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the PostgreSQL database.");
    }
}

// Enable Swagger UI with Swagger 2.0 spec for Nintex/K2 compatibility over HTTP with root basePath '/'
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        var host = httpReq.Host.HasValue ? httpReq.Host.Value : "kyc.runasp.net";
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = $"http://{host}" }
        };
    });
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Account & Document API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

// Custom schema filter to remove 'additionalProperties: false' for K2 compatibility
public class RemoveAdditionalPropertiesSchemaFilter : Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter
{
    public void Apply(OpenApiSchema schema, Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext context)
    {
        schema.AdditionalPropertiesAllowed = true;
        schema.AdditionalProperties = null;
    }
}

// Custom operation filter for K2 SmartObject field mapping
public class K2ResponseFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        var non200Keys = operation.Responses.Keys.Where(k => k != "200").ToList();
        foreach (var key in non200Keys)
        {
            operation.Responses.Remove(key);
        }
    }
}
