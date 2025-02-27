using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
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

        public async Task<IActionResult> Index() // Dodajemy async
        {
            var model = new BedroomViewModel();
            try
            {
                model = await GetPlcVariables(); // Dodajemy await
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
                //model.sTekst = "Błąd odczytu";
            }
            return View(model);
        }

        [Route("bedroom/json")]
        public async Task<IActionResult> Json() // Dodajemy async
        {
            try
            {
                var model = await GetPlcVariables(); // Dodajemy await
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
                return StatusCode(500);
            }
        }

        private async Task<BedroomViewModel> GetPlcVariables() // Dodajemy async Task
        {
            var model = new BedroomViewModel();
            try // Dodajemy try-catch
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
               // model.sTekst = "Błąd odczytu";
            }
            return model;
        }

        [HttpPost("bedroom/toggleBool")]
        public async Task<IActionResult> ToggleBool()
        {
            try
            {
                bool myBoolVariable = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bWallSocketHMI");
                myBoolVariable = !myBoolVariable;
                await _plcService.WriteVariableAsync("P_Bedroom.bWallSocketHMI", myBoolVariable);

                var model = await GetPlcVariables(); // Dodajemy await
                model.wallSocketHMI = myBoolVariable;

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zmiany MyBoolVariable w kontrolerze.");
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