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
        private readonly PlcService _plcService;

        public HomeController(ILogger<HomeController> logger, PlcService plcService)
        {
            _logger = logger;
            _plcService = plcService;
        }

        public IActionResult Index()
        {
            var model = new PlcVariablesViewModel();
            try
            {
                model = GetPlcVariables(); // Pobieramy dane z PLC do modelu
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
                model.sTekst = "Błąd odczytu";
            }
            return View(model);
        }

        [Route("json")]
        public async Task<IActionResult> Json()
        {
            try
            {
                var model = GetPlcVariables(); // Pobieramy dane z PLC do modelu
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
                return StatusCode(500); // Zwracamy kod błędu
            }
        }

        private PlcVariablesViewModel GetPlcVariables() // Funkcja do pobierania danych z PLC
        {
            var model = new PlcVariablesViewModel();
            model.MyBoolVariable = _plcService.ReadVariableAsync<bool>("P_Bedroom.bLampSwitchLeftHMI").Result;
            model.iCounter = _plcService.ReadVariableAsync<int>("MyGVL.iCounter").Result;
            model.sTekst = _plcService.ReadVariableAsync<string>("MyGVL.sTekst").Result;
            model.iTemperature = _plcService.ReadVariableAsync<int>("MyGVL.iTemperature").Result;
            model.iPressure = _plcService.ReadVariableAsync<int>("MyGVL.iPressure").Result;
            model.MomentarySwitch = _plcService.ReadVariableAsync<bool>("MyGVL.MomentarySwitch").Result;
            model.ToggleSwitch = _plcService.ReadVariableAsync<bool>("MyGVL.ToggleSwitch").Result;
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

                var model = GetPlcVariables(); // Pobieramy aktualne dane z PLC
                model.MyBoolVariable = myBoolVariable; // Ustawiamy zaktualizowaną wartość

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

                var model = GetPlcVariables(); // Pobieramy aktualne dane z PLC
                model.iPressure = data.iPressure; // Ustawiamy zaktualizowaną wartość

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zmiany iPressure w kontrolerze.");
                return StatusCode(500);
            }
        }

        [HttpPost("/SetMomentarySwitchToTrue")] // Poprawiona nazwa akcji
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

        [HttpPost("/SetMomentarySwitchToFalse")] // Poprawiona nazwa akcji
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



        public IActionResult Room1()

        {

            return View();

        }



        public IActionResult Room2()

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