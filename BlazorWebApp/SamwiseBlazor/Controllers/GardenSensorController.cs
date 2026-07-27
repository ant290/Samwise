using Microsoft.AspNetCore.Mvc;
using SamwiseBlazor.DatabaseModels;
using SamwiseBlazor.Dtos;
using SamwiseBlazor.Services;

namespace SamwiseBlazor.Controllers;

public class GardenSensorController : ControllerBase
{
    private readonly ILogger<GardenSensorController> _logger;
    private readonly ISensorDataService _sensorDataService;

    public GardenSensorController(ILogger<GardenSensorController> logger, ISensorDataService sensorDataService)
    {
        _logger = logger;
        _sensorDataService = sensorDataService;
    }

    [HttpGet("api/gardensensor")]
    public ActionResult<IEnumerable<SensorData>> GetGardenSensorData()
    {
        var sensorData = _sensorDataService.GetRecent(10);
        return Ok(sensorData);
    }

    [HttpPost("api/gardensensor")]
    public IActionResult PostGardenSensorData([FromBody] GardenSensorData sensorData)
    {
        if (sensorData.DeviceId == 0)
        {
            return BadRequest("DeviceId is required.");
        }

        var newSensorData = new SensorData
        {
            SourceDeviceId = sensorData.DeviceId,
            Timestamp = DateTime.UtcNow
        };

        _sensorDataService.AddSensorData(newSensorData);

        _logger.LogInformation("Garden sensor data received from {SourceDeviceId} at {Timestamp}", newSensorData.SourceDeviceId, newSensorData.Timestamp);

        return Ok(sensorData);
    }
}