using Microsoft.Extensions.Options;
using TwinCAT.Ads;
using System;
using System.Collections.Generic; // Dodaj ten using
using WebApplication2.Models; // Dodaj ten using

namespace PlcVariableReader
{
    public class PlcReader : IDisposable
    {
        private readonly AdsClient _adsClient;
        private bool _disposed = false;
        private readonly string _amsNetId;
        private readonly int _port;

        public PlcReader(IOptions<PlcConfiguration> plcConfiguration)
        {
            _amsNetId = plcConfiguration.Value.IpAddress;
            _port = plcConfiguration.Value.Port;

            if (_amsNetId == null || _port == 0)
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

        public object ReadVariable(string variableName)
        {
            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);
                object value = _adsClient.ReadAny(handle, GetTypeForVariable(variableName)); // Użyj GetTypeForVariable
                _adsClient.DeleteVariableHandle(handle);
                return value;
            }
            catch (Exception ex)
            {
                throw new PlcException($"Błąd odczytu zmiennej '{variableName}': {ex.Message}", ex);
            }
        }

        public void WriteVariable(string variableName, object value)
        {
            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);

                // Dodajemy sprawdzenie typu i konwersję (opcjonalne, ale zalecane)
                if (value != null && value.GetType() == GetTypeForVariable(variableName))
                {
                    _adsClient.WriteAny(handle, value);
                }
                else
                {
                    throw new ArgumentException($"Niepoprawny typ wartości dla zmiennej '{variableName}'. Oczekiwano {GetTypeForVariable(variableName)} a otrzymano {(value != null ? value.GetType() : "null")}.");
                }

                _adsClient.DeleteVariableHandle(handle);
            }
            catch (Exception ex)
            {
                throw new PlcException($"Błąd zapisu zmiennej '{variableName}': {ex.Message}", ex);
            }
        }


        // Dodajemy metodę GetTypeForVariable
        private Type GetTypeForVariable(string variableName)
        {
            if (variableName == "MyGVL.MyBoolVariable")
            {
                return typeof(bool);
            }
            else if (variableName == "MyGVL.iCounter")
            {
                return typeof(int);
            }
            // ... inne zmienne
            else
            {
                return typeof(object); // Lub rzuć wyjątek
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