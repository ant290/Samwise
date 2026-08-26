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

    [HttpGet("api/gardensensor/hello")]
    public ActionResult<string> GetHello()
    {
        _logger.LogInformation("Hello from the Garden Sensor API!");
        return Ok("Hello from the Garden Sensor API!");
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

        // check if the device exists and if not create it
        var existingDevice = _sensorDataService.GetSensorDevice(sensorData.DeviceId);
        if (existingDevice == null)
        {
            var newDevice = new SensorDevice
            {
                Id = sensorData.DeviceId,
                Name = $"Device {sensorData.DeviceId}",
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Location = "Unknown"
            };

            _sensorDataService.AddSensorDevice(newDevice);
            _logger.LogInformation("Created new sensor device with ID {DeviceId}", newDevice.Id);
        } else if (existingDevice.IPAddress != HttpContext.Connection.RemoteIpAddress?.ToString())
        {
            existingDevice.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            _sensorDataService.UpdateSensorDevice(existingDevice);
            _logger.LogInformation("Updated sensor device {DeviceId} IP address to {IPAddress}", existingDevice.Id, existingDevice.IPAddress);
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

            var sensorDetails = _sensorDataService.GetSensorDetails(sensorData.DeviceId, reading.SensorId);
            if (sensorDetails == null)
            {
                _sensorDataService.AddSensorDetails(new SensorDetails
                {
                    SensorDeviceId = sensorData.DeviceId,
                    SensorId = reading.SensorId,
                    SensorType = reading.SensorType
                });
            }
            else if (sensorDetails.SensorType != reading.SensorType)
            {
                sensorDetails.SensorType = reading.SensorType;
                _sensorDataService.UpdateSensorDetails(sensorDetails);
            }
        }

        _logger.LogInformation("Garden sensor data for {SourceDeviceId} at {Timestamp} saved as {SensorDataId}", newSensorData.SourceDeviceId, newSensorData.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"), sensorDataId);

        return Ok(sensorData);
    }
}