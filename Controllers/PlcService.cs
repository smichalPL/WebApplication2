using PlcVariableReader;
using Microsoft.Extensions.Logging; // Dodajemy using dla ILogger

public class PlcService
{
    private readonly PlcReader _plcReader;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly ILogger<PlcService> _logger; // Dodajemy ILogger

    public PlcService(PlcReader plcReader, ILogger<PlcService> logger) // Dodajemy ILogger do konstruktora
    {
        _plcReader = plcReader;
        _logger = logger;
    }

    public async Task<T> ReadVariableAsync<T>(string variableName)
    {
        _logger.LogInformation($"Próba odczytu zmiennej: {variableName}"); // Logujemy próbę odczytu

        await _semaphore.WaitAsync();
        try
        {
            T result = _plcReader.ReadVariable<T>(variableName);
            _logger.LogInformation($"Odczytano wartość {result} z {variableName}"); // Logujemy odczytaną wartość
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Błąd odczytu zmiennej {variableName}: {ex.Message}"); // Logujemy błąd odczytu
            throw; // Ponownie rzucamy wyjątek, aby był widoczny w PlcHub
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task WriteVariableAsync<T>(string variableName, T value)
    {
        _logger.LogInformation($"Próba zapisu wartości {value} do zmiennej: {variableName}"); // Logujemy próbę zapisu

        await _semaphore.WaitAsync();
        try
        {
            _plcReader.WriteVariable<T>(variableName, value);
            _logger.LogInformation($"Zapisano wartość {value} do zmiennej {variableName}"); // Logujemy sukces zapisu
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Błąd zapisu do zmiennej {variableName}: {ex.Message}"); // Logujemy błąd zapisu
            throw; // Ponownie rzucamy wyjątek, aby był widoczny w PlcHub
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<ST_WeeklyTimeSwitchInput[]> ReadWeeklyScheduleAsync(int section)
    {
        string variableName = $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection{section}";
        _logger.LogInformation($"Próba odczytu harmonogramu: {variableName}");

        await _semaphore.WaitAsync();
        try
        {
            var schedule = _plcReader.ReadWeeklySchedule(section);
            _logger.LogInformation($"Odczytano harmonogram {variableName}: {schedule.Length} wpisów.");
            return schedule;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Błąd odczytu harmonogramu {variableName}: {ex.Message}");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}