using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BedroomController : Controller
    {
        private readonly ILogger<BedroomController> _logger;
        private readonly PlcService _plcService;

        public BedroomController(ILogger<BedroomController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new BedroomViewModel();
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

        [Route("bedroom/json")]
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

        private async Task<BedroomViewModel> GetPlcVariables()
        {
            var model = new BedroomViewModel();
            try
            {
                model.lampSwitchLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bLampSwitchLeftHMI");
                model.wallSocketHMI = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bWallSocketHMI");
                model.facadeBlindsUpHMI = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bFacadeBlindsUpHMI");
                model.facadeBlindsDownHMI = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bFacadeBlindsDownHMI");
                model.facadeBlindsStopHMI = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bFacadeBlindsStopHMI");
                model.lampRelayCeiling = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_BedroomLampRelayCeiling");
                model.windowOpenSensor = await _plcService.ReadVariableAsync<bool>("GVL_IO.X_BedroomWindowOpenSensor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables: " + ex.Message);
            }
            return model;
        }

        [HttpPost("bedroom/toggleBool")]
        public async Task<IActionResult> ToggleBool()
        {
            return await ToggleState("P_Bedroom.bWallSocketHMI", "wallSocketHMI");
        }

        [HttpPost("bedroom/toggleBlindsUp")]
        public async Task<IActionResult> ToggleBlindsUp()
        {
            return await ToggleState("P_Bedroom.bFacadeBlindsUpHMI", "facadeBlindsUpHMI");
        }

        [HttpPost("bedroom/toggleBlindsDown")]
        public async Task<IActionResult> ToggleBlindsDown()
        {
            return await ToggleState("P_Bedroom.bFacadeBlindsDownHMI", "facadeBlindsDownHMI");
        }

        [HttpPost("bedroom/toggleBlindsStop")]
        public async Task<IActionResult> ToggleBlindsStop()
        {
            return await ToggleState("P_Bedroom.bFacadeBlindsStopHMI", "facadeBlindsStopHMI");
        }

        private async Task<IActionResult> ToggleState(string plcVariableName, string modelPropertyName)
        {
            try
            {
                bool toggleState = await _plcService.ReadVariableAsync<bool>(plcVariableName);
                toggleState = !toggleState;
                await _plcService.WriteVariableAsync(plcVariableName, toggleState);

                var model = await GetPlcVariables();

                typeof(BedroomViewModel).GetProperty(modelPropertyName)?.SetValue(model, toggleState);

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zmiany {plcVariableName} w kontrolerze.");
                return StatusCode(500);
            }
        }

        [HttpPost("bedroom/SetMomentarySwitchToTrue")]
        public async Task<IActionResult> SetMomentarySwitchToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Bedroom.bLampSwitchLeftHMI", true);
                return Json(new { Message = "MomentarySwitch ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("bedroom/SetMomentarySwitchToFalse")]
        public async Task<IActionResult> SetMomentarySwitchToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Bedroom.bLampSwitchLeftHMI", false);
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