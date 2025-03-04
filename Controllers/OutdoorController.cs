using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class OutdoorController : Controller
    {
        private readonly ILogger<OutdoorController> _logger;
        private readonly PlcService _plcService;

        public OutdoorController(ILogger<OutdoorController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new OutdoorViewModel();
            try
            {
                model = await GetPlcVariables();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze outdoor.");
            }
            return View(model);
        }

        [Route("outdoor/json")]
        public async Task<IActionResult> Json()
        {
            try
            {
                var model = await GetPlcVariables();
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze outdoor.");
                return StatusCode(500);
            }
        }

        private async Task<OutdoorViewModel> GetPlcVariables()
        {
            var model = new OutdoorViewModel();
            try
            {
                model.lampSwitch1HMI = await _plcService.ReadVariableAsync<bool>("P_Outdoor.bLampSwitch1HMI");
                model.lampSwitch2HMI = await _plcService.ReadVariableAsync<bool>("P_Outdoor.bLampSwitch2HMI");
                model.wallSocket1HMI = await _plcService.ReadVariableAsync<bool>("P_Outdoor.bWallSocketFixWindowsHMI");
                model.wallSocket2HMI = await _plcService.ReadVariableAsync<bool>("P_Outdoor.bWallSocketHsHMI");
                model.wallSocket3HMI = await _plcService.ReadVariableAsync<bool>("P_Outdoor.bWallSocketRoofHMI");
                model.lampRelayCeiling1 = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_OutdoorLampHS");
                model.lampRelayCeiling2 = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_OutdoorLampEntrance");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables dla Outdoor: " + ex.Message);
            }
            return model;
        }

        [HttpPost("outdoor/toggleWallSocket1")]
        public async Task<IActionResult> ToggleWallSocket1()
        {
            return await ToggleState("P_Outdoor.bWallSocketFixWindowsHMI", "wallSocket1HMI");
        }

        [HttpPost("outdoor/toggleWallSocket2")]
        public async Task<IActionResult> ToggleWallSocket2()
        {
            return await ToggleState("P_Outdoor.bWallSocketHsHMI", "wallSocket2HMI");
        }

        [HttpPost("outdoor/toggleWallSocket3")]
        public async Task<IActionResult> ToggleWallSocket3()
        {
            return await ToggleState("P_Outdoor.bWallSocketRoofHMI", "wallSocket3HMI");
        }

        private async Task<IActionResult> ToggleState(string plcVariableName, string modelPropertyName)
        {
            try
            {
                bool toggleState = await _plcService.ReadVariableAsync<bool>(plcVariableName);
                toggleState = !toggleState;
                await _plcService.WriteVariableAsync(plcVariableName, toggleState);

                var model = await GetPlcVariables();

                typeof(OutdoorViewModel).GetProperty(modelPropertyName)?.SetValue(model, toggleState);

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zmiany {plcVariableName} w kontrolerze Outdoor.");
                return StatusCode(500);
            }
        }

        [HttpPost("outdoor/SetMomentarySwitch1ToTrue")]
        public async Task<IActionResult> SetMomentarySwitch1ToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Outdoor.bLampSwitch1HMI", true);
                return Json(new { Message = "MomentarySwitch1 ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch1 na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("outdoor/SetMomentarySwitch1ToFalse")]
        public async Task<IActionResult> SetMomentarySwitch1ToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Outdoor.bLampSwitch1HMI", false);
                return Json(new { Message = "MomentarySwitch1 ustawiona na false" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch1 na false: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("outdoor/SetMomentarySwitch2ToTrue")]
        public async Task<IActionResult> SetMomentarySwitch2ToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Outdoor.bLampSwitch2HMI", true);
                return Json(new { Message = "MomentarySwitch2 ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch2 na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("outdoor/SetMomentarySwitch2ToFalse")]
        public async Task<IActionResult> SetMomentarySwitch2ToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Outdoor.bLampSwitch2HMI", false);
                return Json(new { Message = "MomentarySwitch2 ustawiona na false" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch2 na false: {ex.Message}");
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