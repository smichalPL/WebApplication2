using Microsoft.AspNetCore.SignalR;
using PlcVariableReader;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class PlcHub : Hub
{
    private readonly PlcReader _plcReader;
    private readonly ILogger<PlcHub> _logger;

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

    public async Task AktualizujDane(string grupa)
    {
        _logger.LogInformation("Wywołano metodę AktualizujDane.");
        try
        {
            var boolValue = _plcReader.ReadBoolVariable("MyGVL.MyBoolVariable");
            var intValue = _plcReader.ReadIntVariable("MyGVL.iCounter");

            _logger.LogInformation($"Odczytane wartości: MyBoolVariable = {boolValue}, iCounter = {intValue}");

            await Clients.Group(grupa).SendAsync("OtrzymajDane", boolValue, intValue);
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
        await OdlaczOdGrupy("TwojaGrupa"); // Opuść grupę przy rozłączeniu
        await base.OnDisconnectedAsync(exception);
    }
}