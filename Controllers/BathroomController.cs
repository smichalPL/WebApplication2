using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BathroomController : Controller
    {
        private readonly ILogger<BathroomController> _logger;
        private readonly PlcService _plcService;

        public BathroomController(ILogger<BathroomController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new BathroomViewModel();
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

        [Route("bathroom/json")]
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

        private async Task<BathroomViewModel> GetPlcVariables()
        {
            var model = new BathroomViewModel();
            try
            {
                model.lampSwitchHMI = await _plcService.ReadVariableAsync<bool>("P_Bathroom.bLampSwitchHMI");
                model.wallSocketHMI = await _plcService.ReadVariableAsync<bool>("P_Bathroom.bWallSocketHMI");
                model.facadeBlindsUpHMI = await _plcService.ReadVariableAsync<bool>("P_Bathroom.bFacadeBlindsUpHMI");
                model.facadeBlindsDownHMI = await _plcService.ReadVariableAsync<bool>("P_Bathroom.bFacadeBlindsDownHMI");
                model.facadeBlindsStopHMI = await _plcService.ReadVariableAsync<bool>("P_Bathroom.bFacadeBlindsStopHMI");
                model.lampRelayCeiling = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_BathroomLampRelayCeiling");
                model.windowOpenSensor = await _plcService.ReadVariableAsync<bool>("GVL_IO.X_BathroomWindowOpenSensor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables: " + ex.Message);
            }
            return model;
        }

        [HttpPost("bathroom/toggleBool")]
        public async Task<IActionResult> ToggleBool()
        {
            return await ToggleState("P_Bathroom.bWallSocketHMI", "wallSocketHMI");
        }

        [HttpPost("bathroom/toggleBlindsUp")]
        public async Task<IActionResult> ToggleBlindsUp()
        {
            return await ToggleState("P_Bathroom.bFacadeBlindsUpHMI", "facadeBlindsUpHMI");
        }

        [HttpPost("bathroom/toggleBlindsDown")]
        public async Task<IActionResult> ToggleBlindsDown()
        {
            return await ToggleState("P_Bathroom.bFacadeBlindsDownHMI", "facadeBlindsDownHMI");
        }

        [HttpPost("bathroom/toggleBlindsStop")]
        public async Task<IActionResult> ToggleBlindsStop()
        {
            return await ToggleState("P_Bathroom.bFacadeBlindsStopHMI", "facadeBlindsStopHMI");
        }

        private async Task<IActionResult> ToggleState(string plcVariableName, string modelPropertyName)
        {
            try
            {
                bool toggleState = await _plcService.ReadVariableAsync<bool>(plcVariableName);
                toggleState = !toggleState;
                await _plcService.WriteVariableAsync(plcVariableName, toggleState);

                var model = await GetPlcVariables();

                typeof(BathroomController).GetProperty(modelPropertyName)?.SetValue(model, toggleState);

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zmiany {plcVariableName} w kontrolerze.");
                return StatusCode(500);
            }
        }

        [HttpPost("bathroom/SetMomentarySwitchToTrue")]
        public async Task<IActionResult> SetMomentarySwitchToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Bathroom.bLampSwitchHMI", true);
                return Json(new { Message = "MomentarySwitch ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("bathroom/SetMomentarySwitchToFalse")]
        public async Task<IActionResult> SetMomentarySwitchToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Bathroom.bLampSwitchHMI", false);
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
