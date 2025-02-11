using Microsoft.AspNetCore.SignalR;
using PlcVariableReader;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic; // Dodaj ten using
using WebApplication2.Models; // Dodaj using dla modeli

public class PlcHub : Hub
{
    private readonly PlcReader _plcReader;
    private readonly ILogger<PlcHub> _logger;
    private readonly Dictionary<string, Zmienna> _zmienne = new Dictionary<string, Zmienna>(); // Słownik zmiennych

    public PlcHub(PlcReader plcReader, ILogger<PlcHub> logger)
    {
        _plcReader = plcReader;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await DolaczDoGrupy("TwojaGrupa"); // Dołącz do grupy po połączeniu
        await base.OnConnectedAsync();
    }

    public async Task OdczytajZmienne(string grupa, List<string> nazwyZmiennych)
    {
        _logger.LogInformation("Wywołano metodę OdczytajZmienne.");
        var odczytaneZmienne = new Dictionary<string, object>(); // Słownik dla odczytanych wartości

        foreach (var nazwa in nazwyZmiennych)
        {
            try
            {
                var wartosc = _plcReader.ReadVariable(nazwa);
                _zmienne[nazwa] = new Zmienna { Nazwa = nazwa, Wartosc = wartosc }; // Aktualizuj słownik
                odczytaneZmienne.Add(nazwa, wartosc); // Dodaj do słownika wyników
                _logger.LogInformation($"Odczytano zmienną: {nazwa}, wartość: {wartosc}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd odczytu zmiennej: {nazwa}");
                await Clients.Group(grupa).SendAsync("BladOdczytu", nazwa, ex.Message);
                return; // Przerwij pętlę w przypadku błędu
            }
        }
        await Clients.Group(grupa).SendAsync("ZmienneOdczytane", odczytaneZmienne); // Wyślij słownik z wynikami
    }


    public async Task ZapiszZmienne(string grupa, Dictionary<string, object> zmienneDoZapisu)
    {
        _logger.LogInformation("Wywołano metodę ZapiszZmienne.");

        foreach (var kvp in zmienneDoZapisu)
        {
            string nazwa = kvp.Key;
            object wartosc = kvp.Value;

            try
            {
                _plcReader.WriteVariable(nazwa, wartosc);
                _zmienne[nazwa].Wartosc = wartosc; // Aktualizuj słownik
                await Clients.Group(grupa).SendAsync("ZmiennaZapisana", nazwa, wartosc);
                _logger.LogInformation($"Zapisano zmienną: {nazwa}, wartość: {wartosc}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd zapisu zmiennej: {nazwa}");
                await Clients.Group(grupa).SendAsync("BladZapisu", nazwa, ex.Message);
                return; // Przerwij pętlę w przypadku błędu
            }
        }
    }

    public async Task DolaczDoGrupy(string grupa)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, grupa);
        _logger.LogInformation($"Dodano do grupy: {grupa}, ConnectionId: {Context.ConnectionId}");
    }

    public async Task OdlaczOdGrupy(string grupa)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupa);
        _logger.LogInformation($"Odłączono od grupy: {grupa}, ConnectionId: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await OdlaczOdGrupy("TwojaGrupa"); // Opuść grupę przy rozłączeniu
        await base.OnDisconnectedAsync(exception);
    }
}