using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Dodajemy using dla ILogger
using WebApplication2.Models;
using PlcVariableReader;
using System.Text.Json; // Dodajemy using dla JsonSerializer

namespace WebApplication2.Controllers
{
    public class IrrigationController : Controller
    {
        private readonly PlcService _plcService;
        private readonly ILogger<IrrigationController> _logger; // Dodajemy ILogger

        public IrrigationController(PlcService plcService, ILogger<IrrigationController> logger) // Dodajemy ILogger do konstruktora
        {
            _plcService = plcService;
            _logger = logger;
        }

        // Akcja główna – odczytuje tablicę z PLC i mapuje ją na model widoku
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Rozpoczęto akcję Index.");

            try
            {
                List<ST_InnerStruct> plcData = await _plcService.ReadTestArrayAsync();
                _logger.LogInformation($"Index: Odczytano dane z PLC: {JsonSerializer.Serialize(plcData)}");

                var viewModel = plcData.Select((item, index) => new IrrigationViewModel
                {
                    Index = index,
                    bBoolTest1 = item.bBoolTest1,
                    bBoolTest2 = item.bBoolTest2,
                    CzasRaw = item.Czas
                }).ToList();

                _logger.LogInformation($"Index: Model widoku: {JsonSerializer.Serialize(viewModel)}");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji Index.");
                return StatusCode(500, "Wystąpił błąd serwera.");
            }
        }

        // Akcja do aktualizacji pól BOOL (np. bBoolTest1 lub bBoolTest2)
        [HttpPost]
        public async Task<IActionResult> UpdateTestArray([FromBody] UpdateArrayRequest request)
        {
            _logger.LogInformation($"Rozpoczęto akcję UpdateTestArray. Request: {JsonSerializer.Serialize(request)}");

            if (request == null)
            {
                _logger.LogWarning("UpdateTestArray: Otrzymano nieprawidłowe żądanie.");
                return BadRequest("Invalid request");
            }

            try
            {
                string variableName = $"P_IrrigationSystemTmp.stTestArray[{request.Index}].{request.Field}";
                await _plcService.WriteVariableAsync<bool>(variableName, request.Value);
                _logger.LogInformation($"UpdateTestArray: Zapisano wartość {request.Value} do zmiennej {variableName}.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji UpdateTestArray.");
                return StatusCode(500, "Wystąpił błąd serwera.");
            }
        }

        // Oddzielna akcja do aktualizacji pola czasu (Czas)
        [HttpPost]
        public async Task<IActionResult> UpdateTime([FromBody] UpdateTimeRequest request)
        {
            _logger.LogInformation($"Rozpoczęto akcję UpdateTime. Request: {JsonSerializer.Serialize(request)}");

            if (request == null)
            {
                _logger.LogWarning("UpdateTime: Otrzymano nieprawidłowe żądanie.");
                return BadRequest("Invalid request");
            }

            try
            {
                uint milliseconds = (uint)request.Time; // Konwertujemy int na uint
                string variableName = $"P_IrrigationSystemTmp.stTestArray[{request.Index}].Czas";
                await _plcService.WriteVariableAsync<uint>(variableName, milliseconds);
                _logger.LogInformation($"UpdateTime: Zapisano wartość {milliseconds} do zmiennej {variableName}.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji UpdateTime.");
                return StatusCode(500, "Wystąpił błąd serwera.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTestArray()
        {
            _logger.LogInformation("Rozpoczęto akcję GetTestArray.");

            try
            {
                List<ST_InnerStruct> data = await _plcService.ReadTestArrayAsync();

                // Mapowanie na IrrigationViewModel
                var viewModel = data.Select(item => new IrrigationViewModel
                {
                    Index = data.IndexOf(item),
                    CzasRaw = item.Czas,
                    bBoolTest1 = item.bBoolTest1,
                    bBoolTest2 = item.bBoolTest2
                }).ToList();

                _logger.LogInformation($"GetTestArray: Odczytano dane z PLC: {JsonSerializer.Serialize(viewModel)}");
                return Json(viewModel); // Zwracamy ViewModel
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji GetTestArray.");
                return StatusCode(500, "Wystąpił błąd serwera.");
            }
        }

        // Model pomocniczy do aktualizacji pól BOOL
        public class UpdateArrayRequest
        {
            public int Index { get; set; }
            public string Field { get; set; }
            public bool Value { get; set; }
        }

        // Model pomocniczy do aktualizacji pola czasu (Czas)
        public class UpdateTimeRequest
        {
            public int Index { get; set; }
            public int Time { get; set; } // Zmieniamy na int (milisekundy)
        }



    }
}