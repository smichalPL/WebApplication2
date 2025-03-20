using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication2.Models;
using System.Runtime.InteropServices;
using TwinCAT.PlcOpen;

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
            { "GVL_IO.Y_BathroomLampRelayMirror", typeof(bool) },

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
            { "MyGVL.ToggleSwitch", typeof(bool) },
        };

        static PlcReader()
        {
            _plcVariables.Add("P_IrrigationSystem.bValveSwitchHMI", typeof(bool[])); // Dodaj całą tablicę

            for (int i = 0; i <= 6; i++) // Dodaj też pojedyncze elementy
            {
                _plcVariables.Add($"P_IrrigationSystem.bValveSwitchHMI[{i}]", typeof(bool));
            }

            // Definicja pól dla jednego elementu tablicy
            string[] fieldsBool = new string[]
            {
        "bEnable",
        "bSunday",
        "bMonday",
        "bTuesday",
        "bWednesday",
        "bThursday",
        "bFriday",
        "bSaturday"
            };

            string[] fieldsTime = new string[]
            {
        "tTimeOn",
        "tTimeOff"
            };

            // Zakładamy, że mamy 7 sekcji (0-6) i 2 elementy w każdej
            for (int section = 0; section <= 6; section++)
            {
                for (int index = 0; index < 2; index++)
                {
                    string baseKey = $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection{section}[{index}]";
                    // Pola typu bool
                    foreach (var field in fieldsBool)
                    {
                        _plcVariables.Add($"{baseKey}.{field}", typeof(bool));
                    }
                    // Pola czasu – tutaj zakładamy, że zapisujemy jako TimeSpan
                    foreach (var field in fieldsTime)
                    {
                        _plcVariables.Add($"{baseKey}.{field}", typeof(TOD));
                    }
                }
            }
        }

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
            _adsClient.Connect(_amsNetId, _port);

        }

        public T ReadVariable<T>(string variableName)
        {
            if (!_plcVariables.ContainsKey(variableName))
                throw new ArgumentException($"Zmienna '{variableName}' nie istnieje w słowniku.");

            Type expectedType = _plcVariables[variableName];
            if (typeof(T) != expectedType)
                throw new ArgumentException($"Niezgodność typów dla '{variableName}'. Oczekiwano '{expectedType.Name}', a podano '{typeof(T).Name}'.");

            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);
                T value;

                if (typeof(T) == typeof(string))
                {
                    byte[] bytes = _adsClient.ReadAny(handle, typeof(byte[]), new int[] { 51 }) as byte[];
                    value = (T)(object)Encoding.ASCII.GetString(bytes).TrimEnd('\0');
                }
                else
                {
                    value = (T)_adsClient.ReadAny(handle, typeof(T));
                }

                _adsClient.DeleteVariableHandle(handle);
                return value;
            }
            catch (Exception ex)
            {
                throw new PlcException($"Błąd odczytu '{variableName}': {ex.Message}", ex);
            }
        }

        public void WriteVariable<T>(string variableName, T value)
        {
            _logger.LogInformation($"Próba zapisu zmiennej '{variableName}' wartością: {value}");

            if (!_plcVariables.ContainsKey(variableName))
                throw new ArgumentException($"Zmienna '{variableName}' nie istnieje w słowniku.");

            Type expectedType = _plcVariables[variableName];
            _logger.LogInformation($"Typ zmiennej '{variableName}' w słowniku: {expectedType.Name}");

            if (typeof(T) != expectedType)
                throw new ArgumentException($"Niezgodność typów dla '{variableName}'. Oczekiwano '{expectedType.Name}', a podano '{typeof(T).Name}'.");

            try
            {
                var handle = _adsClient.CreateVariableHandle(variableName);

                if (typeof(T) == typeof(string))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(value.ToString());
                    Array.Resize(ref bytes, 51);
                    _adsClient.WriteAny(handle, bytes);
                }
                else
                {
                    _adsClient.WriteAny(handle, value);
                }

                _adsClient.DeleteVariableHandle(handle);
            }
            catch (Exception ex)
            {
                throw new PlcException($"Błąd zapisu '{variableName}': {ex.Message}", ex);
            }
        }


        public List<ST_WeeklyTimeSwitchInput> ReadWeeklyTimeSwitchArray(string arrayVariableName, int arrayLength = 2)
        {
            List<ST_WeeklyTimeSwitchInput> result = new List<ST_WeeklyTimeSwitchInput>();
            int sizeOfStruct = 16; // 8 bajtów na 8 BOOL-ów + 4 + 4 bajty na TOD-y

            for (int i = 0; i < arrayLength; i++)
            {
                try
                {
                    string fullVariableName = $"{arrayVariableName}[{i}]";
                    var handle = _adsClient.CreateVariableHandle(fullVariableName);
                    byte[] data = _adsClient.ReadAny(handle, typeof(byte[]), new int[] { sizeOfStruct }) as byte[];
                    _adsClient.DeleteVariableHandle(handle);

                    if (data != null && data.Length == sizeOfStruct)
                    {
                        _logger.LogInformation($"Surowe dane dla {fullVariableName}: {BitConverter.ToString(data)}");

                        ST_WeeklyTimeSwitchInput input = new ST_WeeklyTimeSwitchInput
                        {
                            bEnable = data[0] != 0,
                            bSunday = data[1] != 0,
                            bMonday = data[2] != 0,
                            bTuesday = data[3] != 0,
                            bWednesday = data[4] != 0,
                            bThursday = data[5] != 0,
                            bFriday = data[6] != 0,
                            bSaturday = data[7] != 0,
                            tTimeOnRaw = BitConverter.ToUInt32(data, 8),
                            tTimeOffRaw = BitConverter.ToUInt32(data, 12)
                        };

                        result.Add(input);
                    }
                    else
                    {
                        _logger.LogError($"Niepoprawna długość danych dla {fullVariableName}. Dane: {BitConverter.ToString(data)}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Błąd odczytu {arrayVariableName}[{i}]: {ex.Message}");
                }
            }

            return result;
        }

        public bool[] ReadBoolArray(string variableName, int length)
        {
            var handle = _adsClient.CreateVariableHandle(variableName);
            try
            {
                // Pobierz handle do zmiennej
                handle = _adsClient.CreateVariableHandle(variableName);

                // Odczytaj tablicę bool
                return (bool[])_adsClient.ReadAny(handle, typeof(bool[]), new int[] { length });
            }
            finally
            {
                if (handle != 0)
                {
                    _adsClient.DeleteVariableHandle(handle); // Zwolnij handle
                }
            }
        }



        public void WriteBoolArray(string variableName, bool[] values)
        {
            var handle = _adsClient.CreateVariableHandle(variableName);
            try
            {
                // Zapisz tablicę bool do PLC
                _adsClient.WriteAny(handle, values);
            }
            finally
            {
                if (handle != 0)
                {
                    _adsClient.DeleteVariableHandle(handle); // Zwolnij handle
                }
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
