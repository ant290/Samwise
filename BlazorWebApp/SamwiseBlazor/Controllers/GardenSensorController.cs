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
        _logger.LogInformation("Received garden sensor data from device {DeviceId} with {ReadingCount} readings", sensorData.DeviceId, sensorData.SensorReadings.Length);

        if (sensorData.DeviceId == 0)
        {
            return BadRequest("DeviceId is required.");
        }

        var newSensorData = new SensorData
        {
            SourceDeviceId = sensorData.DeviceId,
            Timestamp = DateTime.Now
        };

        int sensorDataId = _sensorDataService.AddSensorData(newSensorData);

        foreach (var reading in sensorData.SensorReadings)
        {
            var sensorReading = new SensorReading
            {
                SensorDataId = sensorDataId,
                SensorId = reading.SensorId,
                SensorType = reading.SensorType,
                ValueInt = reading.ValueInt,
                ValueFloat = reading.ValueFloat,
                ValueBool = reading.ValueBool,
                ValueString = reading.ValueString
            };

            _sensorDataService.AddSensorReading(sensorReading);
        }

        _logger.LogInformation("Garden sensor data for {SourceDeviceId} at {Timestamp} saved as {SensorDataId}", newSensorData.SourceDeviceId, newSensorData.Timestamp, sensorDataId);

        return Ok(sensorData);
    }
}