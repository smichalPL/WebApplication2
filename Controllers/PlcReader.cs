using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcVariableReader
{
    public class PlcReader : IDisposable
    {
        private readonly AdsClient _adsClient;
        private bool _disposed = false;
        private readonly string _amsNetId;
        private readonly int _port;
        private readonly ILogger<PlcReader> _logger;

        // Klucze w słowniku MUSZĄ być dokładnie takie same, jak nazwy zmiennych w PLC!
        // Prefiks "MyGVL." ZOSTAŁ dodany z powrotem.
        private static Dictionary<string, Type> _plcVariables = new Dictionary<string, Type>()
        {
            { "P_Bedroom.bLampSwitchLeftHMI", typeof(bool) },
            { "MyGVL.iCounter", typeof(Int32) },
            { "MyGVL.sTekst", typeof(string) },
            { "MyGVL.iTemperature", typeof(Int32) },
            { "MyGVL.iPressure", typeof(Int32) },
            { "MyGVL.MomentarySwitch", typeof(bool) },
            { "MyGVL.ToggleSwitch", typeof(bool) } //P_Bedroom.bLampSwitchLeftHMI
        };

        public PlcReader(IOptions<PlcConfiguration> plcConfiguration, ILogger<PlcReader> logger)
        {
            _amsNetId = plcConfiguration.Value.IpAddress;
            _port = plcConfiguration.Value.Port;
            _logger = logger;

            if (string.IsNullOrEmpty(_amsNetId) || _port == 0)
            {
                _logger.LogError("Adres IP i/lub port PLC nie zostały skonfigurowane.");
                throw new ArgumentNullException("Adres IP i/lub port PLC nie zostały skonfigurowane.");
            }

            _adsClient = new AdsClient();

            try
            {
                _logger.LogInformation($"Próba połączenia z PLC: {_amsNetId}:{_port}");
                _adsClient.Connect(_amsNetId, _port);
                _logger.LogInformation($"Połączono z PLC: {_amsNetId}:{_port}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd połączenia z PLC: {ex.Message}");
                throw new PlcException($"Błąd połączenia z PLC: {ex.Message}", ex);
            }
        }

        public T ReadVariable<T>(string variableName)
        {
            _logger.LogInformation($"PlcReader: Próba odczytu zmiennej: {variableName}");

            if (!_plcVariables.ContainsKey(variableName))
            {
                _logger.LogError($"Zmienna '{variableName}' nie istnieje w słowniku.");
                throw new ArgumentException($"Zmienna '{variableName}' nie istnieje w słowniku.");
            }

            Type expectedType = _plcVariables[variableName];
            if (typeof(T) != expectedType)
            {
                _logger.LogError($"Niezgodność typów dla zmiennej '{variableName}'. Oczekiwano '{expectedType.Name}', a podano '{typeof(T).Name}'.");
                throw new ArgumentException($"Niezgodność typów dla zmiennej '{variableName}'. Oczekiwano '{expectedType.Name}', a podano '{typeof(T).Name}'.");
            }

            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);

                if (typeof(T) == typeof(string))
                {
                    /*byte[] bytes = _adsClient.ReadAny<byte[]>(handle, 51); // Odczytujemy 51 bajtów (STRING(51))
                    string strValue = Encoding.ASCII.GetString(bytes).TrimEnd('\0'); // Konwersja z ASCII
                    _adsClient.DeleteVariableHandle(handle);
                    _logger.LogInformation($"PlcReader: Odczytano wartość '{strValue}' z {variableName}");
                    return (T)(object)strValue;*/
                    return (T)(object)"nie diała!";
                }
                else
                {
                    T value = (T)_adsClient.ReadAny(handle, typeof(T));
                    _adsClient.DeleteVariableHandle(handle);
                    _logger.LogInformation($"PlcReader: Odczytano wartość {value} z {variableName}");
                    return value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PlcReader: Błąd odczytu zmiennej '{variableName}': {ex.Message}");
                throw new PlcException($"Błąd odczytu zmiennej '{variableName}': {ex.Message}", ex);
            }
        }

        public void WriteVariable<T>(string variableName, T value)
        {
            _logger.LogInformation($"PlcReader: Próba zapisu wartości {value} do zmiennej: {variableName}");

            if (!_plcVariables.ContainsKey(variableName))
            {
                _logger.LogError($"Zmienna '{variableName}' nie istnieje w słowniku.");
                throw new ArgumentException($"Zmienna '{variableName}' nie istnieje w słowniku.");
            }

            Type expectedType = _plcVariables[variableName];
            if (typeof(T) != expectedType)
            {
                _logger.LogError($"Niezgodność typów dla zmiennej '{variableName}'. Oczekiwano '{expectedType.Name}', a podano '{typeof(T).Name}'.");
                throw new ArgumentException($"Niezgodność typów dla zmiennej '{variableName}'. Oczekiwano '{expectedType.Name}', a podano '{typeof(T).Name}'.");
            }

            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);

                if (typeof(T) == typeof(string))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(value.ToString()); // Konwersja na ASCII
                    Array.Resize(ref bytes, 51); // Ustalenie rozmiaru na 51 bajtów
                    _adsClient.WriteAny(handle, bytes);
                }
                else
                {
                    _adsClient.WriteAny(handle, value);
                }

                _adsClient.DeleteVariableHandle(handle);
                _logger.LogInformation($"PlcReader: Zapisano wartość {value} do zmiennej: {variableName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PlcReader: Błąd zapisu do zmiennej '{variableName}': {ex.Message}");
                throw new PlcException($"Błąd zapisu do zmiennej '{variableName}': {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _adsClient?.Dispose();
                }

                _disposed = true;
            }
        }

        ~PlcReader()
        {
            Dispose(false);
        }
    }
}