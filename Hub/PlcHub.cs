using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using WebApplication2.Models;
using PlcVariableReader;

public class PlcHub : Hub
{
    private readonly PlcService _plcService;
    private readonly ILogger<PlcHub> _logger;
    private readonly Dictionary<string, Zmienna> _zmienne = new Dictionary<string, Zmienna>(); // Słownik zmiennych

    public PlcHub(PlcService plcService, ILogger<PlcHub> logger)
    {
        _plcService = plcService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            await DolaczDoGrupy("TwojaGrupa");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dołączania do grupy w OnConnectedAsync");
            Context.Abort();
        }

        await base.OnConnectedAsync();
    }

    public async Task OdczytajZmienne(string grupa, List<string> nazwyZmiennych)
    {
        _logger.LogInformation("Wywołano metodę OdczytajZmienne.");
        try
        {
         //   bool boolValue = await _plcService.ReadVariableAsync<bool>("MyGVL.MyBoolVariable");
            int intValue = await _plcService.ReadVariableAsync<int>("MyGVL.iCounter");
          //  bool momentarySwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.MomentarySwitch");
          //  bool toggleSwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.ToggleSwitch");

            _logger.LogInformation($"Odczytane wartości: iCounter = {intValue}");

            await Clients.Group(grupa).SendAsync("OtrzymajDane", intValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odczytu danych z PLC");
            await Clients.Group(grupa).SendAsync("BladOdczytu", ex.Message);
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
        try
        {
            await OdlaczOdGrupy("TwojaGrupa");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odłączania od grupy w OnDisconnectedAsync");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task ZapiszDane(string grupa, string variableName, string viewModelJson)
    {
        _logger.LogInformation($"ZapiszDane: {variableName}, JSON: {viewModelJson}"); // Logujemy odebrany JSON

        try
        {
            if (string.IsNullOrEmpty(viewModelJson))
            {
                _logger.LogWarning($"Próba zapisu wartości null do zmiennej '{variableName}'.");
                await Clients.Caller.SendAsync("BladZapisu", variableName, "Nie można zapisać wartości null.");
                return;
            }

            var viewModel = JsonSerializer.Deserialize<PlcVariablesViewModel>(viewModelJson);
            if (viewModel == null)
            {
                _logger.LogError("Deserializacja JSON nie powiodła się."); // Logujemy błąd deserializacji
                await Clients.Caller.SendAsync("BladZapisu", variableName, "Niepoprawna wartość.");
                return;
            }

            _logger.LogInformation($"Zdeserializowany model: {JsonSerializer.Serialize(viewModel)}"); // Logujemy zdeserializowany model

            switch (variableName)
            {
                case "MyGVL.iPressure":
                    await _plcService.WriteVariableAsync("MyGVL.iPressure", viewModel.iPressure);
                    break;
                case "P_Bedroom.bLampSwitchLeftHMI":
                    await _plcService.WriteVariableAsync("P_Bedroom.bLampSwitchLeftHMI", viewModel.MyBoolVariable);
                    break;
                case "MyGVL.sTekst":
                    await _plcService.WriteVariableAsync("MyGVL.sTekst", viewModel.sTekst);
                    break;
                case "MyGVL.iTemperature":
                    await _plcService.WriteVariableAsync("MyGVL.iTemperature", viewModel.iTemperature);
                    break;
                case "MyGVL.MomentarySwitch":
                    await _plcService.WriteVariableAsync("MyGVL.MomentarySwitch", viewModel.MomentarySwitch);
                    break;
                case "MyGVL.ToggleSwitch":
                    await _plcService.WriteVariableAsync("MyGVL.ToggleSwitch", viewModel.ToggleSwitch);
                    break;
                case "MyGVL.iCounter":
                    await _plcService.WriteVariableAsync("MyGVL.iCounter", viewModel.iCounter);
                    break;
                default:
                    _logger.LogWarning($"Nieobsługiwana zmienna: {variableName}");
                    await Clients.Caller.SendAsync("BladZapisu", variableName, "Nieobsługiwana zmienna.");
                    return;
            }

            _logger.LogInformation($"Zapisano wartość do zmiennej '{variableName}'.");
            await Clients.Group(grupa).SendAsync("DaneZapisane", variableName, viewModel);
        }
        catch (JsonException ex) // Dodajemy catch dla JsonException
        {
            _logger.LogError(ex, $"Błąd deserializacji JSON: {ex.Message}");
            await Clients.Caller.SendAsync("BladZapisu", variableName, $"Błąd deserializacji: {ex.Message}");
        }
        catch (PlcException ex) // Dodajemy catch dla PlcException (jeśli taki istnieje)
        {
            _logger.LogError(ex, $"Błąd zapisu do PLC: {ex.Message}");
            await Clients.Caller.SendAsync("BladZapisu", variableName, $"Błąd zapisu do PLC: {ex.Message}");
        }
        catch (Exception ex) // Ogólny catch (łapie wszystkie inne wyjątki)
        {
            _logger.LogError(ex, $"Błąd w ZapiszDane: {ex.Message}");
            await Clients.Caller.SendAsync("BladZapisu", variableName, $"Wystąpił błąd: {ex.Message}");
        }
    }
}