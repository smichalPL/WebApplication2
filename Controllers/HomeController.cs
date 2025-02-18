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
                model.MyBoolVariable = _plcService.ReadVariableAsync<bool>("MyGVL.MyBoolVariable").Result;
                model.iCounter = _plcService.ReadVariableAsync<int>("MyGVL.iCounter").Result;
                model.sTekst = _plcService.ReadVariableAsync<string>("MyGVL.sTekst").Result;
                model.iTemperature = _plcService.ReadVariableAsync<int>("MyGVL.iTemperature").Result;
                model.iPressure = _plcService.ReadVariableAsync<int>("MyGVL.iPressure").Result;
                model.MomentarySwitch = _plcService.ReadVariableAsync<bool>("MyGVL.MomentarySwitch").Result;
                model.ToggleSwitch = _plcService.ReadVariableAsync<bool>("MyGVL.ToggleSwitch").Result;
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
            var model = new PlcVariablesViewModel();
            try
            {
                model.MyBoolVariable = await _plcService.ReadVariableAsync<bool>("MyGVL.MyBoolVariable");
                model.iCounter = await _plcService.ReadVariableAsync<int>("MyGVL.iCounter");
                model.sTekst = await _plcService.ReadVariableAsync<string>("MyGVL.sTekst");
                model.iTemperature = await _plcService.ReadVariableAsync<int>("MyGVL.iTemperature");
                model.iPressure = await _plcService.ReadVariableAsync<int>("MyGVL.iPressure");
                model.MomentarySwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.MomentarySwitch");
                model.ToggleSwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.ToggleSwitch");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC w kontrolerze.");
                model.sTekst = "Błąd odczytu";
            }

            var data = new
            {
                MyBoolVariable = model.MyBoolVariable,
                iCounter = model.iCounter,
                sTekst = model.sTekst,
                iTemperature = model.iTemperature,
                iPressure = model.iPressure,
                MomentarySwitch = model.MomentarySwitch,
                ToggleSwitch = model.ToggleSwitch
            };
            return Json(data);
        }

        [HttpPost("/toggleBool")]
        public async Task<IActionResult> ToggleBool()
        {
            try
            {
                bool myBoolVariable = await _plcService.ReadVariableAsync<bool>("MyGVL.MyBoolVariable");
                myBoolVariable = !myBoolVariable;
                await _plcService.WriteVariableAsync("MyGVL.MyBoolVariable", myBoolVariable);

                var model = new PlcVariablesViewModel();
                model.MyBoolVariable = myBoolVariable;
                model.iCounter = await _plcService.ReadVariableAsync<int>("MyGVL.iCounter");
                model.sTekst = await _plcService.ReadVariableAsync<string>("MyGVL.sTekst");
                model.iTemperature = await _plcService.ReadVariableAsync<int>("MyGVL.iTemperature");
                model.iPressure = await _plcService.ReadVariableAsync<int>("MyGVL.iPressure");
                model.MomentarySwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.MomentarySwitch");
                model.ToggleSwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.ToggleSwitch");

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas zmiany MyBoolVariable w kontrolerze.");
                return StatusCode(500);
            }
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