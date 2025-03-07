using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Dodajemy using dla ILogger
using WebApplication2.Models;
using PlcVariableReader;
using System.Text.Json; // Dodajemy using dla JsonSerializer

namespace WebApplication2.Controllers
{
    public class IrrigationController : Controller
    {
        private readonly PlcService _plcService;
        private readonly ILogger<IrrigationController> _logger; // Dodajemy ILogger

        public IrrigationController(PlcService plcService, ILogger<IrrigationController> logger) // Dodajemy ILogger do konstruktora
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
                // Odczytujemy dane dla poszczególnych sekcji; zakładamy, że każda tablica ma 2 elementy (ARRAY[0..1])
                var section0 = await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection0", 2);
                var section1 = await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection1", 2);
                var section2 = await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection2", 2);
                var section3 = await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection3", 2);
                var section4 = await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection4", 2);
                var section5 = await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection5", 2);
                var section6 = await _plcService.ReadWeeklyTimeSwitchArrayAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection6", 2);

                var viewModel = new WeeklyTimeSwitchCompositeViewModel
                {
                    Section0 = section0.Select((item, index) => new WeeklyTimeSwitchInputViewModel
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
                        tTimeOn = item.tTimeOn,
                        tTimeOff = item.tTimeOff
                    }).ToList(),
                    Section1 = section1.Select((item, index) => new WeeklyTimeSwitchInputViewModel
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
                        tTimeOn = item.tTimeOn,
                        tTimeOff = item.tTimeOff
                    }).ToList(),
                    Section2 = section2.Select((item, index) => new WeeklyTimeSwitchInputViewModel
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
                        tTimeOn = item.tTimeOn,
                        tTimeOff = item.tTimeOff
                    }).ToList(),
                    Section3 = section3.Select((item, index) => new WeeklyTimeSwitchInputViewModel
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
                        tTimeOn = item.tTimeOn,
                        tTimeOff = item.tTimeOff
                    }).ToList(),
                    Section4 = section4.Select((item, index) => new WeeklyTimeSwitchInputViewModel
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
                        tTimeOn = item.tTimeOn,
                        tTimeOff = item.tTimeOff
                    }).ToList(),
                    Section5 = section5.Select((item, index) => new WeeklyTimeSwitchInputViewModel
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
                        tTimeOn = item.tTimeOn,
                        tTimeOff = item.tTimeOff
                    }).ToList(),
                    Section6 = section6.Select((item, index) => new WeeklyTimeSwitchInputViewModel
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
                        tTimeOn = item.tTimeOn,
                        tTimeOff = item.tTimeOff
                    }).ToList(),
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

        public class UpdateWeeklyTimeSwitchRequest
        {
            public int Section { get; set; } // Numer sekcji (0 do 6)
            public int Index { get; set; }
            public string Field { get; set; }
            public bool Value { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWeeklyTimeSwitch([FromBody] UpdateWeeklyTimeSwitchRequest request)
        {
            _logger.LogInformation($"Rozpoczęto akcję UpdateWeeklyTimeSwitch. Request: {JsonSerializer.Serialize(request)}");

            if (request == null)
            {
                _logger.LogWarning("UpdateWeeklyTimeSwitch: Otrzymano nieprawidłowe żądanie.");
                return BadRequest("Invalid request");
            }

            try
            {
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

                await _plcService.WriteVariableAsync<bool>(variableName, request.Value);
                _logger.LogInformation($"UpdateWeeklyTimeSwitch: Zapisano wartość {request.Value} do zmiennej {variableName}.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji UpdateWeeklyTimeSwitch.");
                return StatusCode(500, "Wystąpił błąd serwera.");
            }
        }
    }
}