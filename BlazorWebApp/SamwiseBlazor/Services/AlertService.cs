using SamwiseBlazor.DatabaseModels;
using SamwiseBlazor.Dtos;

namespace SamwiseBlazor.Services;

public interface IAlertService
{
    List<SensorAlert> GetAlerts();
}

public class AlertService : IAlertService
{
    private readonly ISensorDataService _sensorDataService;
    private readonly ILogger<AlertService> _logger;

    public AlertService(ISensorDataService sensorDataService, ILogger<AlertService> logger)
    {
        _sensorDataService = sensorDataService;
        _logger = logger;
    }

    public List<SensorAlert> GetAlerts()
    {
        var alerts = new List<SensorAlert>();

        foreach (var device in _sensorDataService.GetSensorDevices())
        {
            var latestReadings = _sensorDataService.GetLatestReadings(device.Id)
                .ToDictionary(reading => reading.SensorId);

            foreach (var sensorDetails in _sensorDataService.GetSensorDetails(device.Id))
            {
                if (!HasAlertValue(sensorDetails) || !latestReadings.TryGetValue(sensorDetails.SensorId, out var latestReading))
                {
                    continue;
                }

                if (latestReading.SensorType != sensorDetails.SensorType)
                {
                    // Log a warning or handle the mismatch as needed
                    _logger.LogWarning("Sensor type mismatch for SensorId {SensorId} on DeviceId {DeviceId}. Latest reading type: {LatestType}, Sensor details type: {DetailsType}",
                        sensorDetails.SensorId, device.Id, latestReading.SensorType, sensorDetails.SensorType);
                    continue;
                }

                if (!IsAlertTriggered(sensorDetails, latestReading))
                {
                    continue;
                }

                alerts.Add(new SensorAlert
                {
                    SensorDeviceId = device.Id,
                    SensorDeviceName = device.Name,
                    SensorId = sensorDetails.SensorId,
                    SensorType = sensorDetails.SensorType,
                    SensorDescription = sensorDetails.Description,
                    AlertInt = sensorDetails.AlertInt,
                    AlertFloat = sensorDetails.AlertFloat,
                    AlertBool = sensorDetails.AlertBool,
                    AlertString = sensorDetails.AlertString,
                    LastReading = latestReading
                });
            }
        }

        return alerts;
    }

    private static bool HasAlertValue(SensorDetails sensorDetails)
    {
        return sensorDetails.AlertInt.HasValue ||
               sensorDetails.AlertFloat.HasValue ||
               sensorDetails.AlertBool.HasValue ||
               !string.IsNullOrWhiteSpace(sensorDetails.AlertString);
    }

    private static bool IsAlertTriggered(SensorDetails sensorDetails, FlatSensorReading latestReading)
    {
        switch (sensorDetails.SensorType)
        {
            case SensorType.SoilMoisture:
                return sensorDetails.AlertInt.HasValue && latestReading.ValueInt.HasValue && latestReading.ValueInt.Value > sensorDetails.AlertInt.Value;
            case SensorType.Temperature:
            case SensorType.Humidity:
                return sensorDetails.AlertFloat.HasValue && latestReading.ValueFloat.HasValue && latestReading.ValueFloat.Value > sensorDetails.AlertFloat.Value;
            case SensorType.Battery:
                return sensorDetails.AlertFloat.HasValue && latestReading.ValueFloat.HasValue && latestReading.ValueFloat.Value < sensorDetails.AlertFloat.Value;
            default:
                return false;
        }
    }
}