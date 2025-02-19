using PlcVariableReader;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja logowania
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
// builder.Logging.AddFile("logs/mylog-.txt", rollingInterval: RollingInterval.Day); // Opcjonalnie

// Rejestracja PlcConfiguration
builder.Services.Configure<PlcConfiguration>(builder.Configuration.GetSection("PlcConfiguration"));

// Rejestracja PlcReader
builder.Services.AddTransient<PlcReader>();

// Rejestracja PlcService
builder.Services.AddTransient<PlcService>();

// Rejestracja SignalR
//builder.Services.AddSignalR();

// Dodajemy us³ugi MVC *i konfigurujemy JSON options*
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // <-- Dodajemy tê liniê
    });

var app = builder.Build();

// Konfiguracja HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapowanie Hubu
//app.MapHub<PlcHub>("/plchub");

app.Run();