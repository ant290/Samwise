using SamwiseBlazor.DatabaseModels;

namespace SamwiseBlazor.Dtos;

public class GardenSensorReading {
    /// <summary>
    /// This ID is set by the ESP32 board and is used to identify which sensor the reading came from.
    /// </summary>
    public int SensorId { get; set; }
    public SensorType SensorType { get; set; }
    public int? ValueInt { get; set; }
    public float? ValueFloat { get; set; }
    public bool? ValueBool { get; set; }
    public string? ValueString { get; set; }
}
