using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class OlekController : Controller
    {
        private readonly ILogger<OlekController> _logger;
        private readonly PlcService _plcService;

        public OlekController(ILogger<OlekController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new OlekViewModel();
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

        [Route("olek/json")]
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

        private async Task<OlekViewModel> GetPlcVariables()
        {
            var model = new OlekViewModel();
            try
            {
                model.lampSwitchLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Olek.bLampSwitchLeftHMI");
                model.wallSocketHMI = await _plcService.ReadVariableAsync<bool>("P_Olek.bWallSocketHMI");
                model.facadeBlindsUpHMI = await _plcService.ReadVariableAsync<bool>("P_Olek.bFacadeBlindsUpHMI");
                model.facadeBlindsDownHMI = await _plcService.ReadVariableAsync<bool>("P_Olek.bFacadeBlindsDownHMI");
                model.facadeBlindsStopHMI = await _plcService.ReadVariableAsync<bool>("P_Olek.bFacadeBlindsStopHMI");
                model.lampRelayCeiling = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_OlekLampRelayCeiling");
                model.windowOpenSensor = await _plcService.ReadVariableAsync<bool>("GVL_IO.X_OlekWindowOpenSensor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables: " + ex.Message);
            }
            return model;
        }

        [HttpPost("olek/toggleBool")]
        public async Task<IActionResult> ToggleBool()
        {
            return await ToggleState("P_Olek.bWallSocketHMI", "wallSocketHMI");
        }

        [HttpPost("olek/toggleBlindsUp")]
        public async Task<IActionResult> ToggleBlindsUp()
        {
            return await ToggleState("P_Olek.bFacadeBlindsUpHMI", "facadeBlindsUpHMI");
        }

        [HttpPost("olek/toggleBlindsDown")]
        public async Task<IActionResult> ToggleBlindsDown()
        {
            return await ToggleState("P_Olek.bFacadeBlindsDownHMI", "facadeBlindsDownHMI");
        }

        [HttpPost("olek/toggleBlindsStop")]
        public async Task<IActionResult> ToggleBlindsStop()
        {
            return await ToggleState("P_Olek.bFacadeBlindsStopHMI", "facadeBlindsStopHMI");
        }

        private async Task<IActionResult> ToggleState(string plcVariableName, string modelPropertyName)
        {
            try
            {
                bool toggleState = await _plcService.ReadVariableAsync<bool>(plcVariableName);
                toggleState = !toggleState;
                await _plcService.WriteVariableAsync(plcVariableName, toggleState);

                var model = await GetPlcVariables();

                typeof(OlekViewModel).GetProperty(modelPropertyName)?.SetValue(model, toggleState);

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zmiany {plcVariableName} w kontrolerze.");
                return StatusCode(500);
            }
        }

        [HttpPost("olek/SetMomentarySwitchToTrue")]
        public async Task<IActionResult> SetMomentarySwitchToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Olek.bLampSwitchLeftHMI", true);
                return Json(new { Message = "MomentarySwitch ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("olek/SetMomentarySwitchToFalse")]
        public async Task<IActionResult> SetMomentarySwitchToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Olek.bLampSwitchLeftHMI", false);
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