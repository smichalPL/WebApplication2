using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class KitchenController : Controller
    {
        private readonly ILogger<KitchenController> _logger;
        private readonly PlcService _plcService;

        public KitchenController(ILogger<KitchenController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new KitchenViewModel();
            try
            {
                model = await GetPlcVariables();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze Kitchen.");
            }
            return View(model);
        }

        [Route("kitchen/json")]
        public async Task<IActionResult> Json()
        {
            try
            {
                var model = await GetPlcVariables();
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze Kitchen.");
                return StatusCode(500);
            }
        }

        private async Task<KitchenViewModel> GetPlcVariables()
        {
            var model = new KitchenViewModel();
            try
            {
                model.lampSwitchHMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bLampSwitchHMI");
                model.wallSocket1HMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bWallSocket1HMI");
                model.wallSocket2HMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bWallSocket2HMI");
                model.facadeBlindsUpLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bFacadeBlindsUpLeftHMI");
                model.facadeBlindsDownLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bFacadeBlindsDownLeftHMI");
                model.facadeBlindsStopLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bFacadeBlindsStopLeftHMI");
                model.facadeBlindsUpRightHMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bFacadeBlindsUpRightHMI");
                model.facadeBlindsDownRightHMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bFacadeBlindsDownRightHMI");
                model.facadeBlindsStopRightHMI = await _plcService.ReadVariableAsync<bool>("P_Kitchen.bFacadeBlindsStopRightHMI");
                model.lampRelayCeiling = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_KitchenLampRelayCeiling");
                model.windowOpenSensor = await _plcService.ReadVariableAsync<bool>("GVL_IO.X_KitchenWindowOpenSensor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables dla Kitchen: " + ex.Message);
            }
            return model;
        }

        [HttpPost("kitchen/toggleWallSocket1")]
        public async Task<IActionResult> ToggleWallSocket1()
        {
            return await ToggleState("P_Kitchen.bWallSocket1HMI", "wallSocket1HMI");
        }

        [HttpPost("kitchen/toggleWallSocket2")]
        public async Task<IActionResult> ToggleWallSocket2()
        {
            return await ToggleState("P_Kitchen.bWallSocket2HMI", "wallSocket2HMI");
        }

        [HttpPost("kitchen/toggleBlindsUpLeft")]
        public async Task<IActionResult> ToggleBlindsUpLeft()
        {
            return await ToggleState("P_Kitchen.bFacadeBlindsUpLeftHMI", "facadeBlindsUpLeftHMI");
        }

        [HttpPost("kitchen/toggleBlindsDownLeft")]
        public async Task<IActionResult> ToggleBlindsDownLeft()
        {
            return await ToggleState("P_Kitchen.bFacadeBlindsDownLeftHMI", "facadeBlindsDownLeftHMI");
        }

        [HttpPost("kitchen/toggleBlindsStopLeft")]
        public async Task<IActionResult> ToggleBlindsStopLeft()
        {
            return await ToggleState("P_Kitchen.bFacadeBlindsStopLeftHMI", "facadeBlindsStopLeftHMI");
        }

        [HttpPost("kitchen/toggleBlindsUpRight")]
        public async Task<IActionResult> ToggleBlindsUpRight()
        {
            return await ToggleState("P_Kitchen.bFacadeBlindsUpRightHMI", "facadeBlindsUpRightHMI");
        }

        [HttpPost("kitchen/toggleBlindsDownRight")]
        public async Task<IActionResult> ToggleBlindsDownRight()
        {
            return await ToggleState("P_Kitchen.bFacadeBlindsDownRightHMI", "facadeBlindsDownRightHMI");
        }

        [HttpPost("kitchen/toggleBlindsStopRight")]
        public async Task<IActionResult> ToggleBlindsStopRight()
        {
            return await ToggleState("P_Kitchen.bFacadeBlindsStopRightHMI", "facadeBlindsStopRightHMI");
        }

        private async Task<IActionResult> ToggleState(string plcVariableName, string modelPropertyName)
        {
            try
            {
                bool toggleState = await _plcService.ReadVariableAsync<bool>(plcVariableName);
                toggleState = !toggleState;
                await _plcService.WriteVariableAsync(plcVariableName, toggleState);

                var model = await GetPlcVariables();

                typeof(KitchenViewModel).GetProperty(modelPropertyName)?.SetValue(model, toggleState);

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zmiany {plcVariableName} w kontrolerze Kitchen.");
                return StatusCode(500);
            }
        }

        [HttpPost("kitchen/SetMomentarySwitchToTrue")]
        public async Task<IActionResult> SetMomentarySwitchToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Kitchen.bLampSwitchHMI", true);
                return Json(new { Message = "MomentarySwitch ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("kitchen/SetMomentarySwitchToFalse")]
        public async Task<IActionResult> SetMomentarySwitchToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Kitchen.bLampSwitchHMI", false);
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