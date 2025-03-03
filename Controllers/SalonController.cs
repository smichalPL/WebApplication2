using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class SalonController : Controller
    {
        private readonly ILogger<SalonController> _logger;
        private readonly PlcService _plcService;

        public SalonController(ILogger<SalonController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new SalonViewModel();
            try
            {
                model = await GetPlcVariables();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze Salon.");
            }
            return View(model);
        }

        [Route("salon/json")]
        public async Task<IActionResult> Json()
        {
            try
            {
                var model = await GetPlcVariables();
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze Salon.");
                return StatusCode(500);
            }
        }

        private async Task<SalonViewModel> GetPlcVariables()
        {
            var model = new SalonViewModel();
            try
            {
                model.lampSwitch1HMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bLampSwitch1HMI");
                model.lampSwitch2HMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bLampSwitch2HMI");
                model.lampSwitch3HMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bLampSwitch3HMI");
                model.wallSocket1HMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bWallSocket1HMI");
                model.wallSocket2HMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bWallSocket2HMI");
                model.wallSocket3HMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bWallSocket3HMI");
                model.wallSocket4HMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bWallSocket4HMI");
                model.facadeBlindsUpHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsUpHMI");
                model.facadeBlindsDownHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsDownHMI");
                model.facadeBlindsStopHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsStopHMI");
                model.facadeBlindsUpHsLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsUpHSLeftHMI");
                model.facadeBlindsDownHsLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsDownHSLeftHMI");
                model.facadeBlindsStopHsLeftHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsStopHSleftHMI");
                model.facadeBlindsUpHsRightHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsUpHSRightHMI");
                model.facadeBlindsDownHsRightHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsDownHSRightHMI");
                model.facadeBlindsStopHsRightHMI = await _plcService.ReadVariableAsync<bool>("P_Salon.bFacadeBlindsStopHSRightHMI");
                model.lampRelayCeiling1 = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_SalonLampRelayCeiling_1");
                model.lampRelayCeiling2 = await _plcService.ReadVariableAsync<bool>("GVL_IO.Y_SalonLampRelayCeiling_2");
                model.windowOpenSensor = await _plcService.ReadVariableAsync<bool>("GVL_IO.X_SalonWindowOpenSensor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables dla Salon: " + ex.Message);
            }
            return model;
        }

        [HttpPost("salon/toggleWallSocket1")]
        public async Task<IActionResult> ToggleWallSocket1()
        {
            return await ToggleState("P_Salon.bWallSocket1HMI", "wallSocket1HMI");
        }

        [HttpPost("salon/toggleWallSocket2")]
        public async Task<IActionResult> ToggleWallSocket2()
        {
            return await ToggleState("P_Salon.bWallSocket2HMI", "wallSocket2HMI");
        }

        [HttpPost("salon/toggleWallSocket3")]
        public async Task<IActionResult> ToggleWallSocket3()
        {
            return await ToggleState("P_Salon.bWallSocket3HMI", "wallSocket3HMI");
        }

        [HttpPost("salon/toggleWallSocket4")]
        public async Task<IActionResult> ToggleWallSocket4()
        {
            return await ToggleState("P_Salon.bWallSocket4HMI", "wallSocket4HMI");
        }

        [HttpPost("salon/toggleBlindsUp")]
        public async Task<IActionResult> ToggleBlindsUp()
        {
            return await ToggleState("P_Salon.bFacadeBlindsUpHMI", "facadeBlindsUpHMI");
        }

        [HttpPost("salon/toggleBlindsDown")]
        public async Task<IActionResult> ToggleBlindsDown()
        {
            return await ToggleState("P_Salon.bFacadeBlindsDownHMI", "facadeBlindsDownHMI");
        }

        [HttpPost("salon/toggleBlindsStop")]
        public async Task<IActionResult> ToggleBlindsStop()
        {
            return await ToggleState("P_Salon.bFacadeBlindsStopHMI", "facadeBlindsStopHMI");
        }

        [HttpPost("salon/toggleBlindsUpHSLeft")]
        public async Task<IActionResult> ToggleBlindsUpHSLeft()
        {
            return await ToggleState("P_Salon.bFacadeBlindsUpHSLeftHMI", "facadeBlindsUpHSLeftHMI");
        }

        [HttpPost("salon/toggleBlindsDownHSLeft")]
        public async Task<IActionResult> ToggleBlindsDownHSLeft()
        {
            return await ToggleState("P_Salon.bFacadeBlindsDownHSLeftHMI", "facadeBlindsDownHSLeftHMI");
        }

        [HttpPost("salon/toggleBlindsStopHSLeft")]
        public async Task<IActionResult> ToggleBlindsStopHSLeft()
        {
            return await ToggleState("P_Salon.bFacadeBlindsStopHSleftHMI", "facadeBlindsStopHSLeftHMI");
        }

        [HttpPost("salon/toggleBlindsUpHSRight")]
        public async Task<IActionResult> ToggleBlindsUpHSRight()
        {
            return await ToggleState("P_Salon.bFacadeBlindsUpHSRightHMI", "facadeBlindsUpHSRightHMI");
        }

        [HttpPost("salon/toggleBlindsDownHSRight")]
        public async Task<IActionResult> ToggleBlindsDownHSRight()
        {
            return await ToggleState("P_Salon.bFacadeBlindsDownHSRightHMI", "facadeBlindsDownHSRightHMI");
        }

        [HttpPost("salon/toggleBlindsStopHSRight")]
        public async Task<IActionResult> ToggleBlindsStopHSRight()
        {
            return await ToggleState("P_Salon.bFacadeBlindsStopHSRightHMI", "facadeBlindsStopHSRightHMI");
        }

        private async Task<IActionResult> ToggleState(string plcVariableName, string modelPropertyName)
        {
            try
            {
                bool toggleState = await _plcService.ReadVariableAsync<bool>(plcVariableName);
                toggleState = !toggleState;
                await _plcService.WriteVariableAsync(plcVariableName, toggleState);

                var model = await GetPlcVariables();

                typeof(SalonViewModel).GetProperty(modelPropertyName)?.SetValue(model, toggleState);

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas zmiany {plcVariableName} w kontrolerze Salon.");
                return StatusCode(500);
            }
        }

        [HttpPost("salon/SetMomentarySwitch1ToTrue")]
        public async Task<IActionResult> SetMomentarySwitch1ToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Salon.bLampSwitch1HMI", true);
                return Json(new { Message = "MomentarySwitch1 ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch1 na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("salon/SetMomentarySwitch1ToFalse")]
        public async Task<IActionResult> SetMomentarySwitch1ToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Salon.bLampSwitch1HMI", false);
                return Json(new { Message = "MomentarySwitch1 ustawiona na false" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch1 na false: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("salon/SetMomentarySwitch2ToTrue")]
        public async Task<IActionResult> SetMomentarySwitch2ToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Salon.bLampSwitch2HMI", true);
                return Json(new { Message = "MomentarySwitch2 ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch2 na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("salon/SetMomentarySwitch2ToFalse")]
        public async Task<IActionResult> SetMomentarySwitch2ToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Salon.bLampSwitch2HMI", false);
                return Json(new { Message = "MomentarySwitch2 ustawiona na false" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch2 na false: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("salon/SetMomentarySwitch3ToTrue")]
        public async Task<IActionResult> SetMomentarySwitch3ToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Salon.bLampSwitch3HMI", true);
                return Json(new { Message = "MomentarySwitch3 ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch3 na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("salon/SetMomentarySwitch3ToFalse")]
        public async Task<IActionResult> SetMomentarySwitch3ToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("P_Salon.bLampSwitch3HMI", false);
                return Json(new { Message = "MomentarySwitch3 ustawiona na false" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch3 na false: {ex.Message}");
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