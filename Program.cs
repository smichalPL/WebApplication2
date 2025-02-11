using PlcVariableReader;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja logowania (bez zmian)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Dodajemy rejestracjê PlcReader - **TO JEST KLUCZOWE**
builder.Services.Configure<PlcConfiguration>(builder.Configuration.GetSection("PlcConfiguration")); // Rejestrujemy konfiguracjê
builder.Services.AddTransient<PlcReader>();  // Dodajemy PlcReader do kontenera DI

// Dodajemy SignalR - **NAJPIERW REJESTRACJA US£UG!**
builder.Services.AddSignalR();

// Add services to the container. (bez zmian)
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline. (bez zmian)
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

// Mapujemy Hub na URL - **DOPIERO PO REJESTRACJI US£UG!**
app.MapHub<PlcHub>("/plchub"); // To musi byæ po app.UseRouting()

app.Run();