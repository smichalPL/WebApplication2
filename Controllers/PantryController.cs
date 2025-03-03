using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class PantryController : Controller
    {
        private readonly ILogger<PantryController> _logger;
        private readonly PlcService _plcService;

        public PantryController(ILogger<PantryController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new PantryViewModel();
            try
            {
                model = await GetPlcVariables();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
            }
            return View(model);
        }

        [Route("pantry/json")]
        public async Task<IActionResult> Json()
        {
            try
            {
                var model = await GetPlcVariables();
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
                return StatusCode(500);
            }
        }

        private async Task<PantryViewModel> GetPlcVariables()
        {
            var model = new PantryViewModel();
            try
            {
                model.lampSwitchHMI = await _plcService.ReadVariableAsync<bool>("P_Pantry.bLampSwitchHMI");
                model.wallSocketHMI = await _plcService.ReadVariableAsync<bool>("P_Pantry.bWallSocketHMI");
                model.facadeBlindsUpHMI = await _plcService.ReadVariableAsync<bool>("P_Pantry.bFacadeBlindsUpHMI");
                model.facadeBlindsDownHMI = await _plcService.ReadVariableAsync<bool>("P_Pantry.bFacadeBlindsDownHMI");
                model.facadeBlindsStopHMI = await _plcService.ReadVariableAsync<bool>("P_Pantry.bFacadeBlindsStopHMI");
                model.lampRelayCeiling = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_PantryLampRelayCeiling");
                model.windowOpenSensor = await _plcService.ReadVariableAsync<bool>("GVL_IO.X_PantryWindowOpenSensor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables: " + ex.Message);
            }
            return model;
        }

        [HttpPost("pantry/toggleBool")]
        public async Task<IActionResult> ToggleBool()
        {
            return await ToggleState("P_Pantry.bWallSocketHMI", "wallSocketHMI");
        }

        [HttpPost("pantry/toggleBlindsUp")]
        public async Task<IActionResult> ToggleBlindsUp()
        {
            return await ToggleState("P_Pantry.bFacadeBlindsUpHMI", "facadeBlindsUpHMI");
        }

        [HttpPost("pantry/toggleBlindsDown")]
        public async Task<IActionResult> ToggleBlindsDown()
        {
            return await ToggleState("P_Pantry.bFacadeBlindsDownHMI", "facadeBlindsDownHMI");
        }

        [HttpPost("pantry/toggleBlindsStop")]
        public async Task<IActionResult> ToggleBlindsStop()
        {
            return await ToggleState("P_Pantry.bFacadeBlindsStopHMI", "facadeBlindsStopHMI");
        }

        private async Task<IActionResult> ToggleState(string plcVariableName, string modelPropertyName)
        {
            try
            {
                bool toggleState = await _plcService.ReadVariableAsync<bool>(plcVariableName);
                toggleState = !toggleState;
                await _plcService.WriteVariableAsync(plcVariableName, toggleState);

                var model = await GetPlcVariables();

                typeof(PantryController).GetProperty(modelPropertyName)?.SetValue(model, toggleState);

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zmiany {plcVariableName} w kontrolerze.");
                return StatusCode(500);
            }
        }

        [HttpPost("pantry/SetMomentarySwitchToTrue")]
        public async Task<IActionResult> SetMomentarySwitchToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Pantry.bLampSwitchHMI", true);
                return Json(new { Message = "MomentarySwitch ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("pantry/SetMomentarySwitchToFalse")]
        public async Task<IActionResult> SetMomentarySwitchToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Pantry.bLampSwitchHMI", false);
                return Json(new { Message = "MomentarySwitch ustawiona na false" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na false: {ex.Message}");
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
