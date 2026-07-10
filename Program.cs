using MoaediPharamcy.Repositories;
using MoaediPharamcy.Services;
using MudBlazor.Services;
using System.Data;
using Microsoft.Data.SqlClient;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor services
builder.Services.AddMudServices();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

// Repositories
builder.Services.AddScoped<IDailySalesRepository, DailySalesRepository>();

// Services
builder.Services.AddScoped<IDailySalesService, DailySalesService>();

// Add Controllers
builder.Services.AddControllers();

// Add API endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "مؤسسه دارویی معدی - API",
        Version = "v1",
        Description = "API برای گزارش‌های فروش روزانه",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "تیم توسعه",
            Email = "support@moaedi.ir"
        }
    });
});

// Add HttpClient for API calls if needed
builder.Services.AddHttpClient();

// Localization (اختیاری - برای پشتیبانی از فارسی)
builder.Services.AddLocalization();

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add Logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "مؤسسه دارویی معدی - API v1");
        options.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseRouting();

// Enable CORS
app.UseCors("AllowAll");

// Map Controllers
app.MapControllers();

// Map Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
