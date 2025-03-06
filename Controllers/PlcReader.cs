﻿using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication2.Models;
using System.Runtime.InteropServices;

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
            { "MyGVL.ToggleSwitch", typeof(bool) },

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
            if (!_plcVariables.ContainsKey(variableName))
                throw new ArgumentException($"Zmienna '{variableName}' nie istnieje w słowniku.");

            Type expectedType = _plcVariables[variableName];
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

        public List<ST_InnerStruct> ReadTestArray()
        {
            List<ST_InnerStruct> result = new List<ST_InnerStruct>();
            int sizeOfStruct = 2; // Rozmiar ST_InnerStruct w bajtach (2 bool'e)

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    // Utworzenie uchwytu dla elementu tablicy w PLC
                    uint handle = _adsClient.CreateVariableHandle($"P_IrrigationSystemTmp.stTestArray[{i}]");

                    // Odczyt danych z PLC za pomocą uchwytu
                    byte[] data = _adsClient.ReadAny(handle, typeof(byte[]), new int[] { sizeOfStruct }) as byte[];

                    // Zwolnienie uchwytu po odczycie
                    _adsClient.DeleteVariableHandle(handle);

                    if (data != null && data.Length == sizeOfStruct)
                    {
                        ST_InnerStruct innerStruct = new ST_InnerStruct
                        {
                            bBoolTest1 = (data[0] != 0),
                            bBoolTest2 = (data[1] != 0)
                        };
                        result.Add(innerStruct);
                    }
                    else
                    {
                        _logger.LogError($"Błąd odczytu stTestArray[{i}]. Otrzymano nieprawidłowe dane.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Błąd odczytu stTestArray[{i}]: {ex.Message}");
                }
            }
            return result;
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
