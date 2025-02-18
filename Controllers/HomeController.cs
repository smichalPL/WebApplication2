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

        public IActionResult Index()
        {
            var model = new PlcVariablesViewModel(); // Utwórz *nową* instancję modelu!

            // **Kluczowe:** Odczytaj wartości z PLC i zaktualizuj model *PRZED* przekazaniem go do widoku.
            // UWAGA: Użycie .Result blokuje wątek i jest akceptowalne TYLKO w akcji kontrolera.
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
                // Możesz opcjonalnie ustawić domyślne wartości w modelu w przypadku błędu.
                model.iCounter = -1; // przykład
                model.sTekst = "Błąd odczytu"; // przykład
            }

            return View(model); // Przekaż model do widoku!
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