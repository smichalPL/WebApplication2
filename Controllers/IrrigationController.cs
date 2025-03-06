using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using PlcVariableReader;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    [HttpPost]
    public async Task<IActionResult> UpdateTestArray([FromBody] UpdateArrayRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request");
        }

        string variableName = $"P_IrrigationSystemTmp.stTestArray[{request.Index}].{request.Field}";
        await _plcService.WriteVariableAsync<bool>(variableName, request.Value);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetTestArray()
    {
        List<ST_InnerStruct> data = await _plcService.ReadTestArrayAsync();
        return Json(data);
    }

    public class UpdateArrayRequest
    {
        public int Index { get; set; }
        public string Field { get; set; }
        public bool Value { get; set; }
    }
}