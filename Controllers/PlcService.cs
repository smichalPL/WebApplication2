using PlcVariableReader;
using WebApplication2.Models;
using Microsoft.Extensions.Logging; // Dodajemy using dla ILogger
using System.Text.Json; // Dodajemy using dla JsonSerializer

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

     public async Task<List<ST_WeeklyTimeSwitchInput>> ReadWeeklyTimeSwitchArrayAsync(string arrayVariableName, int arrayLength = 2)
    {
        _logger.LogInformation($"Próba odczytu tablicy: {arrayVariableName}");
        await _semaphore.WaitAsync();
        try
        {
            var result = _plcReader.ReadWeeklyTimeSwitchArray(arrayVariableName, arrayLength);
            _logger.LogInformation($"Odczytano tablicę {arrayVariableName}: {JsonSerializer.Serialize(result)}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Błąd odczytu tablicy {arrayVariableName}: {ex.Message}");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }


}