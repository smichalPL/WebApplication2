﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication2.Models;
using PlcVariableReader;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

namespace WebApplication2.Controllers
{
    public class IrrigationController : Controller
    {
        private readonly PlcService _plcService;
        private readonly ILogger<IrrigationController> _logger;

        public IrrigationController(PlcService plcService, ILogger<IrrigationController> logger)
        {
            _plcService = plcService;
            _logger = logger;
        }

        // Akcja główna – odczytuje tablicę z PLC i mapuje ją na model widoku
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Rozpoczęto akcję Index dla WeeklyTimeSwitchInput.");

            try
            {
                // Odczytujemy dane dla poszczególnych sekcji
                var sections = new[]
                {
                    await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection0", 2),
                    await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection1", 2),
                    await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection2", 2),
                    await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection3", 2),
                    await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection4", 2),
                    await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection5", 2),
                    await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection6", 2)
                };

                var viewModel = new WeeklyTimeSwitchCompositeViewModel
                {
                    Section0 = sections[0].Select((item, index) => MapToViewModel(item, index)).ToList(),
                    Section1 = sections[1].Select((item, index) => MapToViewModel(item, index)).ToList(),
                    Section2 = sections[2].Select((item, index) => MapToViewModel(item, index)).ToList(),
                    Section3 = sections[3].Select((item, index) => MapToViewModel(item, index)).ToList(),
                    Section4 = sections[4].Select((item, index) => MapToViewModel(item, index)).ToList(),
                    Section5 = sections[5].Select((item, index) => MapToViewModel(item, index)).ToList(),
                    Section6 = sections[6].Select((item, index) => MapToViewModel(item, index)).ToList(),
                };

                _logger.LogInformation($"Index: Odczytano dane z PLC: {JsonSerializer.Serialize(viewModel)}");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji Index.");
                return StatusCode(500, "Wystąpił błąd serwera.");
            }
        }

        // Mapowanie danych do ViewModel
        private WeeklyTimeSwitchInputViewModel MapToViewModel(ST_WeeklyTimeSwitchInput item, int index)
        {
            return new WeeklyTimeSwitchInputViewModel
            {
                Index = index,
                bEnable = item.bEnable,
                bSunday = item.bSunday,
                bMonday = item.bMonday,
                bTuesday = item.bTuesday,
                bWednesday = item.bWednesday,
                bThursday = item.bThursday,
                bFriday = item.bFriday,
                bSaturday = item.bSaturday,
                tTimeOn = item.tTimeOn,  // Użyj właściwości TimeSpan
                tTimeOff = item.tTimeOff // Użyj właściwości TimeSpan
            };
        }


        public class UpdateWeeklyTimeSwitchRequest
        {
            [JsonPropertyName("section")]
            public int Section { get; set; }

            [JsonPropertyName("index")]
            public int Index { get; set; }

            [JsonPropertyName("field")]
            public string Field { get; set; }

            [JsonPropertyName("value")]
            public JsonElement Value { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWeeklyTimeSwitch([FromBody] UpdateWeeklyTimeSwitchRequest request)
        {
            using (var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
            {
                var rawJson = await reader.ReadToEndAsync();
                _logger.LogInformation($"Otrzymano surowe dane JSON: {rawJson}");
            }

            if (request == null)
            {
                return BadRequest(new { error = "Invalid request" });
            }

            try
            {
                _logger.LogInformation($"UpdateWeeklyTimeSwitch: Otrzymano request: {JsonSerializer.Serialize(request)}");

                string variableName = request.Section switch
                {
                    0 => $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection0[{request.Index}].{request.Field}",
                    1 => $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection1[{request.Index}].{request.Field}",
                    2 => $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection2[{request.Index}].{request.Field}",
                    3 => $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection3[{request.Index}].{request.Field}",
                    4 => $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection4[{request.Index}].{request.Field}",
                    5 => $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection5[{request.Index}].{request.Field}",
                    6 => $"P_IrrigationSystem.arrWeeklyTimeSwitchInputSection6[{request.Index}].{request.Field}",
                    _ => throw new InvalidOperationException("Invalid section")
                };

                _logger.LogInformation($"Budowanie nazwy zmiennej: {variableName}");

                if (request.Field.StartsWith("b")) // Obsługa pól BOOL
                {
                    if (request.Value.ValueKind == JsonValueKind.True || request.Value.ValueKind == JsonValueKind.False)
                    {
                        bool boolValue = request.Value.GetBoolean(); // Pobranie wartości BOOL
                        _logger.LogInformation($"Zapisuję bool {boolValue} do zmiennej {variableName}");
                        await _plcService.WriteVariableAsync<bool>(variableName, boolValue);
                        _logger.LogInformation($"UpdateWeeklyTimeSwitch: Zapisano bool {boolValue} do zmiennej {variableName}.");
                    }
                    else
                    {
                        _logger.LogWarning("UpdateWeeklyTimeSwitch: Nieprawidłowy format bool.");
                        return BadRequest(new { error = "Invalid bool format" });
                    }
                }
                else // Zakładamy, że pozostałe pola to czas
                {
                    if (request.Value.ValueKind == JsonValueKind.String && request.Value.TryGetDateTime(out DateTime dateTime))
                    {
                        TimeSpan time = dateTime.TimeOfDay;
                        uint todValue = (uint)time.TotalSeconds;
                        _logger.LogInformation($"Zapisuję czas {todValue} do zmiennej {variableName}");
                        await _plcService.WriteVariableAsync<uint>(variableName, todValue);
                        _logger.LogInformation($"UpdateWeeklyTimeSwitch: Zapisano czas {todValue} do zmiennej {variableName}.");
                    }

                    else
                    {
                        _logger.LogWarning("UpdateWeeklyTimeSwitch: Nieprawidłowy format czasu.");
                        return BadRequest(new { error = "Invalid time format" });
                    }
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji UpdateWeeklyTimeSwitch.");
                return StatusCode(500, new { error = "Wystąpił błąd serwera." });
            }
        }
    }
}
