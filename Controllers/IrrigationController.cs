using Microsoft.AspNetCore.Mvc;

public class IrrigationController : Controller
{
    private readonly PlcService _plcService;

    public IrrigationController(PlcService plcService)
    {
        _plcService = plcService;
    }

    public async Task<IActionResult> Index()
    {
        var testArray = await _plcService.ReadTestArrayAsync();
        return View(testArray);
    }
}