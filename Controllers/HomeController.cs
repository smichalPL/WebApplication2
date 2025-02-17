using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlcVariableReader;
using System.Diagnostics;
using System.Threading.Tasks; // Dodajemy using dla Task
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PlcService _plcService; // Wstrzykujemy PlcService

        public HomeController(ILogger<HomeController> logger, PlcService plcService) // Dodajemy PlcService do konstruktora
        {
            _logger = logger;
            _plcService = plcService; // Przypisujemy wstrzyknięty obiekt
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new PlcVariablesViewModel(); // Utworzenie instancji modelu

                // Odczyt wszystkich zmiennych
                viewModel.MyBoolVariable = await _plcService.ReadVariableAsync<bool>("MyGVL.MyBoolVariable");
                viewModel.iCounter = await _plcService.ReadVariableAsync<int>("MyGVL.iCounter");
                viewModel.sTekst = await _plcService.ReadVariableAsync<string>("MyGVL.sTekst");
                viewModel.iTemperature = await _plcService.ReadVariableAsync<int>("MyGVL.iTemperature");
                viewModel.iPressure = await _plcService.ReadVariableAsync<int>("MyGVL.iPressure");
                viewModel.MomentarySwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.MomentarySwitch");
                viewModel.ToggleSwitch = await _plcService.ReadVariableAsync<bool>("MyGVL.ToggleSwitch");

                return View(viewModel); // Przekazanie modelu do widoku
            }
            catch (PlcException ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC.");
                ViewData["ErrorMessage"] = ex.Message;
                var emptyModel = new PlcVariablesViewModel(); // Utwórz pusty model
                return View(emptyModel); // Przekaż pusty model do widoku
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