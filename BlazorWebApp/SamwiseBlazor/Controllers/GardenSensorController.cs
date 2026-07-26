using Microsoft.AspNetCore.Mvc;
using SamwiseBlazor.Models;

namespace SamwiseBlazor.Controllers;

public class GardenSensorController : ControllerBase
{
    private readonly ILogger<GardenSensorController> _logger;

    public GardenSensorController(ILogger<GardenSensorController> logger)
    {
        _logger = logger;
    }

    [HttpGet("api/gardensensor")]
    public ActionResult<GardenSensorData> GetGardenSensorData()
    {
        // Simulate fetching garden sensor data
        var sensorData = new GardenSensorData
        {
            SoilMoisture = "45"
        };

        return Ok(sensorData);
    }

    [HttpPost("api/gardensensor")]
    public IActionResult PostGardenSensorData([FromBody] GardenSensorData sensorData)
    {
        // Simulate saving garden sensor data
        _logger.LogInformation("Garden sensor data received: {SoilMoisture}", sensorData.SoilMoisture);

        return Ok();
    }
}