using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PlcService _plcService; // Upewnij się, że masz ten serwis

        public HomeController(ILogger<HomeController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public async Task<IActionResult> Index() // Dodajemy async
        {
            var model = new PlcVariablesViewModel();
            try
            {
                model = await GetPlcVariables(); // Dodajemy await
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
                model.sTekst = "Błąd odczytu";
            }
            return View(model);
        }

        [Route("json")]
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

        private async Task<PlcVariablesViewModel> GetPlcVariables() // Dodajemy async Task
        {
            var model = new PlcVariablesViewModel();
            try // Dodajemy try-catch
            {
                model.MyBoolVariable = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bLampSwitchLeftHMI");
                model.iCounter = await _plcService.ReadVariableAsync<int>("MyGVL.iCounter");
                model.sTekst = await _plcService.ReadVariableAsync<string>("MyGVL.sTekst");
                model.iTemperature = await _plcService.ReadVariableAsync<int>("MyGVL.iTemperature");
                model.iPressure = await _plcService.ReadVariableAsync<int>("MyGVL.iPressure");
                model.MomentarySwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.MomentarySwitch");
                model.ToggleSwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.ToggleSwitch");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w GetPlcVariables: " + ex.Message);
                model.sTekst = "Błąd odczytu";
            }
            return model;
        }

        [HttpPost("/toggleBool")]
        public async Task<IActionResult> ToggleBool()
        {
            try
            {
                bool myBoolVariable = await _plcService.ReadVariableAsync<bool>("P_Bedroom.bLampSwitchLeftHMI");
                myBoolVariable = !myBoolVariable;
                await _plcService.WriteVariableAsync("P_Bedroom.bLampSwitchLeftHMI", myBoolVariable);

                var model = await GetPlcVariables(); // Dodajemy await
                model.MyBoolVariable = myBoolVariable;

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zmiany MyBoolVariable w kontrolerze.");
                return StatusCode(500);
            }
        }

        [HttpPost("/updatePressure")]
        public async Task<IActionResult> UpdatePressure([FromBody] PressureData data)
        {
            try
            {
                await _plcService.WriteVariableAsync("MyGVL.iPressure", data.iPressure);

                var model = await GetPlcVariables(); // Dodajemy await
                model.iPressure = data.iPressure;

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zmiany iPressure w kontrolerze.");
                return StatusCode(500);
            }
        }

        [HttpPost("/SetMomentarySwitchToTrue")]
        public async Task<IActionResult> SetMomentarySwitchToTrue()
        {
            try
            {
                await _plcService.WriteVariableAsync("MyGVL.MomentarySwitch", true);
                return Json(new { Message = "MomentarySwitch ustawiona na true" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na true: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("/SetMomentarySwitchToFalse")]
        public async Task<IActionResult> SetMomentarySwitchToFalse()
        {
            try
            {
                await _plcService.WriteVariableAsync("MyGVL.MomentarySwitch", false);
                return Json(new { Message = "MomentarySwitch ustawiona na false" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd podczas ustawiania MomentarySwitch na false: {ex.Message}");
                return StatusCode(500);
            }
        }

        public class PressureData
        {
            public int iPressure { get; set; }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Bedroom()
        {
            return View();
        }

        public IActionResult Bathroom()
        {
            return View();
        }

        public IActionResult Irrigation()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}