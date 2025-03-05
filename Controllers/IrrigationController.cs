using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Models;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    public class IrrigationController : Controller
    {
        private readonly ILogger<IrrigationController> _logger;
        private readonly PlcService _plcService;

        public IrrigationController(ILogger<IrrigationController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Ładowanie strony głównej systemu nawadniania...");
            var model = await GetPlcVariables();
            return View(model);
        }

        [Route("irrigation/json")]
        public async Task<IActionResult> Json()
        {
            _logger.LogInformation("Pobieranie danych w formacie JSON...");
            var model = await GetPlcVariables();
            return Json(model);
        }

        private async Task<IrrigationSystemSettings> GetPlcVariables()
        {
            _logger.LogInformation("Rozpoczęcie odczytu zmiennych z PLC...");
            var model = new IrrigationSystemSettings();

            try
            {
                model.Section1 = await _plcService.ReadVariableAsync<ST_WeeklyTimeSwitchInput[]>("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection1");
                model.Section2 = await _plcService.ReadVariableAsync<ST_WeeklyTimeSwitchInput[]>("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection2");
                model.Section3 = await _plcService.ReadVariableAsync<ST_WeeklyTimeSwitchInput[]>("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection3");
                model.Section4 = await _plcService.ReadVariableAsync<ST_WeeklyTimeSwitchInput[]>("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection4");
                model.Section5 = await _plcService.ReadVariableAsync<ST_WeeklyTimeSwitchInput[]>("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection5");
                model.Section6 = await _plcService.ReadVariableAsync<ST_WeeklyTimeSwitchInput[]>("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection6");
                model.ValveSwitches = await _plcService.ReadVariableAsync<bool[]>("P_IrrigationSystem.bValveSwitchHMI");

                _logger.LogInformation("Pomyślnie odczytano zmienne z PLC.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables: {Message}", ex.Message);
            }

            return model;
        }

        [HttpPost("irrigation/saveSettings")]
        public async Task<IActionResult> SaveSettings([FromBody] IrrigationSystemSettings settings)
        {
            if (settings == null)
            {
                _logger.LogError("Otrzymano NULL w settings.");
                return BadRequest("Błąd: Brak danych wejściowych.");
            }

            _logger.LogInformation("Otrzymano dane do zapisu: {@Settings}", settings);

            try
            {
                await _plcService.WriteVariableAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection1", settings.Section1);
                await _plcService.WriteVariableAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection2", settings.Section2);
                await _plcService.WriteVariableAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection3", settings.Section3);
                await _plcService.WriteVariableAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection4", settings.Section4);
                await _plcService.WriteVariableAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection5", settings.Section5);
                await _plcService.WriteVariableAsync("P_IrrigationSystem.arrWeeklyTimeSwitchInputSection6", settings.Section6);
                await _plcService.WriteVariableAsync("P_IrrigationSystem.bValveSwitchHMI", settings.ValveSwitches);

                _logger.LogInformation("Pomyślnie zapisano ustawienia nawadniania.");
                return Json(new { Message = "Ustawienia zapisane" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zapisu ustawień: {Message}", ex.Message);
                return StatusCode(500);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
