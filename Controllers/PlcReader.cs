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
        private static Dictionary<string, Type> _plcVariables = new Dictionary<string, Type>()
        {
            { "P_Bedroom.bLampSwitchLeftHMI", typeof(bool) },
            { "P_Bedroom.bWallSocketHMI", typeof(bool) },
            { "P_Bedroom.bFacadeBlindsUpHMI", typeof(bool) },
            { "P_Bedroom.bFacadeBlindsDownHMI", typeof(bool) },
            { "P_Bedroom.bFacadeBlindsStopHMI", typeof(bool) },
            { "GVL_IO.X_BedroomWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_BedroomLampRelayCeiling", typeof(bool) },
            
            { "P_Bathroom.bLampSwitchHMI", typeof(bool) },
            { "P_Bathroom.bWallSocketHMI", typeof(bool) },
            { "P_Bathroom.bFacadeBlindsUpHMI", typeof(bool) },
            { "P_Bathroom.bFacadeBlindsDownHMI", typeof(bool) },
            { "P_Bathroom.bFacadeBlindsStopHMI", typeof(bool) },
            { "GVL_IO.X_BathroomWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_BathroomLampRelayCeiling", typeof(bool) },

            { "P_Olek.bLampSwitchLeftHMI", typeof(bool) },
            { "P_Olek.bWallSocketHMI", typeof(bool) },
            { "P_Olek.bFacadeBlindsUpHMI", typeof(bool) },
            { "P_Olek.bFacadeBlindsDownHMI", typeof(bool) },
            { "P_Olek.bFacadeBlindsStopHMI", typeof(bool) },
            { "GVL_IO.X_OlekWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_OlekLampRelayCeiling", typeof(bool) },

            { "P_Leon.bLampSwitchLeftHMI", typeof(bool) },
            { "P_Leon.bWallSocketHMI", typeof(bool) },
            { "P_Leon.bFacadeBlindsUpHMI", typeof(bool) },
            { "P_Leon.bFacadeBlindsDownHMI", typeof(bool) },
            { "P_Leon.bFacadeBlindsStopHMI", typeof(bool) },
            { "GVL_IO.X_LeonWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_LeonLampRelayCeiling", typeof(bool) },

            { "P_Kitchen.bLampSwitchHMI", typeof(bool) },
            { "P_Kitchen.bWallSocket1HMI", typeof(bool) },
            { "P_Kitchen.bWallSocket2HMI", typeof(bool) },
            { "P_Kitchen.bFacadeBlindsUpLeftHMI", typeof(bool) },
            { "P_Kitchen.bFacadeBlindsDownLeftHMI", typeof(bool) },
            { "P_Kitchen.bFacadeBlindsStopLeftHMI", typeof(bool) },
            { "P_Kitchen.bFacadeBlindsUpRightHMI", typeof(bool) },
            { "P_Kitchen.bFacadeBlindsDownRightHMI", typeof(bool) },
            { "P_Kitchen.bFacadeBlindsStopRightHMI", typeof(bool) },
            { "GVL_IO.X_KitchenWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_KitchenLampRelayCeiling", typeof(bool) },

            { "P_Salon.bLampSwitch1HMI", typeof(bool) },
            { "P_Salon.bLampSwitch2HMI", typeof(bool) },
            { "P_Salon.bLampSwitch3HMI", typeof(bool) },
            { "P_Salon.bWallSocket1HMI", typeof(bool) },
            { "P_Salon.bWallSocket2HMI", typeof(bool) },
            { "P_Salon.bWallSocket3HMI", typeof(bool) },
            { "P_Salon.bWallSocket4HMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsUpHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsDownHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsStopHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsUpHSLeftHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsDownHSLeftHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsStopHSleftHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsUpHSRightHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsDownHSRightHMI", typeof(bool) },
            { "P_Salon.bFacadeBlindsStopHSRightHMI", typeof(bool) },
            { "GVL_IO.X_SalonWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_SalonLampRelayCeiling_1", typeof(bool) },
            { "GVL_IO.Y_SalonLampRelayCeiling_2", typeof(bool) },

            { "P_Vestibule.bLampSwitchHMI", typeof(bool) },
            { "GVL_IO.X_VestibuleWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_VestibuleLampRelayCeiling", typeof(bool) },

            { "P_Pantry.bLampSwitchHMI", typeof(bool) },
            { "P_Pantry.bWallSocketHMI", typeof(bool) },
            { "P_Pantry.bFacadeBlindsUpHMI", typeof(bool) },
            { "P_Pantry.bFacadeBlindsDownHMI", typeof(bool) },
            { "P_Pantry.bFacadeBlindsStopHMI", typeof(bool) },
            { "GVL_IO.X_PantryWindowOpenSensor", typeof(bool) },
            { "GVL_IO.Y_PantryLampRelayCeiling", typeof(bool) },

            { "P_Outdoor.bLampSwitch1HMI", typeof(bool) },
            { "P_Outdoor.bLampSwitch2HMI", typeof(bool) },
            { "P_Outdoor.bWallSocketFixWindowsHMI", typeof(bool) },
            { "P_Outdoor.bWallSocketHsHMI", typeof(bool) },
            { "P_Outdoor.bWallSocketRoofHMI", typeof(bool) },
            { "GVL_IO.Y_OutdoorLampHS", typeof(bool) },
            { "GVL_IO.Y_OutdoorLampEntrance", typeof(bool) },

            { "MyGVL.MyBoolVariable", typeof(bool) },
            { "MyGVL.iCounter", typeof(Int32) },
            { "MyGVL.sTekst", typeof(string) },
            { "MyGVL.iTemperature", typeof(Int32) },
            { "MyGVL.iPressure", typeof(Int32) },
            { "MyGVL.MomentarySwitch", typeof(bool) },
            { "MyGVL.ToggleSwitch", typeof(bool) }
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