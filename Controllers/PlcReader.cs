using Microsoft.Extensions.Options; // Dodaj ten using
using TwinCAT.Ads; // Dodaj ten using (lub odpowiedni dla twojej biblioteki ADS)
using System;

namespace PlcVariableReader
{
    public class PlcReader : IDisposable
    {
        private readonly AdsClient _adsClient;
        private bool _disposed = false;
        private readonly string _amsNetId;
        private readonly int _port;

        public PlcReader(IOptions<PlcConfiguration> plcConfiguration) // Wstrzykiwanie IOptions
        {
            _amsNetId = plcConfiguration.Value.IpAddress;
            _port = plcConfiguration.Value.Port;

            if (_amsNetId == null || _port == 0) // Sprawdzamy, czy konfiguracja jest poprawna
            {
                throw new ArgumentNullException("Adres IP i/lub port PLC nie zostały skonfigurowane.");
            }

            _adsClient = new AdsClient();

            try
            {
                _adsClient.Connect(_amsNetId, _port);
            }
            catch (Exception ex)
            {
                throw new PlcException($"Błąd połączenia z PLC: {ex.Message}", ex);
            }
        }


        public bool ReadBoolVariable(string variableName)
        {
            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);
                bool value = (bool)_adsClient.ReadAny(handle, typeof(bool));
                _adsClient.DeleteVariableHandle(handle);
                return value;
            }
            catch (Exception ex)
            {
                throw new PlcException($"Błąd odczytu zmiennej '{variableName}': {ex.Message}", ex);
            }
        }
        public int ReadIntVariable(string variableName)
        {
            Console.WriteLine($"Próba odczytu zmiennej: {variableName}"); // Log

            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);
                Console.WriteLine($"Utworzono handle dla zmiennej: {variableName}"); // Log
                int value = (int)_adsClient.ReadAny(handle, typeof(int));
                _adsClient.DeleteVariableHandle(handle);
                Console.WriteLine($"Odczytano wartość zmiennej '{variableName}': {value}"); // Log
                return value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu zmiennej '{variableName}': {ex.Message}"); // Log
                throw new PlcException($"Błąd odczytu zmiennej '{variableName}': {ex.Message}", ex);
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

    public class PlcException : Exception
    {
        public PlcException(string message, Exception innerException) : base(message, innerException) { }
    }
}