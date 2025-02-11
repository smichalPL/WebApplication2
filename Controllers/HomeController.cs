using Microsoft.AspNetCore.Mvc;
using PlcVariableReader;
using System.Diagnostics;
using WebApplication2.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging; // Dodajemy using dla ILogger

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Dodajemy ILogger
        private readonly PlcReader _plcReader; // Poprawne pole: readonly i inicjalizowane w konstruktorze

        public HomeController(ILogger<HomeController> logger, PlcReader plcReader) // Wstrzykujemy PlcReader
        {
            _logger = logger;
            _plcReader = plcReader; // Przypisujemy wstrzyknięty obiekt
        }

        public IActionResult Index()
        {
            if (_plcReader == null)  // Sprawdzamy, czy _plcReader jest zainicjalizowane
            {
                _logger.LogError("PlcReader jest null. Sprawdź rejestrację w DI."); // Logujemy błąd
                ViewData["ErrorMessage"] = "Błąd: Nie można połączyć z PLC."; // Komunikat dla użytkownika
                return View();
            }

            try
            {
                object boolValue = _plcReader.ReadVariable("MyGVL.MyBoolVariable");
                bool myBoolValue = boolValue != null ? (bool)boolValue : false; // Domyślślna wartość false

                object intValue = _plcReader.ReadVariable("MyGVL.iCounter");
                int myIntValue = intValue != null ? (int)intValue : 0; // Domyślna wartość 0

                ViewData["MyBoolValue"] = myBoolValue;
                ViewData["MyIntValue"] = myIntValue;

                _logger.LogInformation("Odczytano wartości z PLC: MyBoolValue={myBoolValue}, MyIntValue={myIntValue}", myBoolValue, myIntValue);

                return View();
            }
            catch (PlcException ex)
            {
                _logger.LogError(ex, "Błąd odczytu z PLC."); // Logujemy wyjątek
                ViewData["ErrorMessage"] = ex.Message; // Przekazujemy komunikat o błędzie do widoku
                return View();
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