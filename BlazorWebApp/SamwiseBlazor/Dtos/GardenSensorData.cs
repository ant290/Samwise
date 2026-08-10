namespace SamwiseBlazor.Dtos;

public class GardenSensorData {
    public int DeviceId { get; set; }
    public GardenSensorReading[] SensorReadings { get; set; }
}
