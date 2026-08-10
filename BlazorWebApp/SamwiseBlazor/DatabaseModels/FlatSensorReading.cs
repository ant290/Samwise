namespace SamwiseBlazor.DatabaseModels;

public class FlatSensorReading
{
    public int SourceDeviceId { get; set; }
    public string? SensorDeviceName { get; set; }
    public DateTime TimeStamp { get; set; }
    public int SensorId { get; set; }
    public SensorType SensorType { get; set; }
    public int? ValueInt { get; set; }
    public bool? ValueBool { get; set; }
    public string? ValueString { get; set; }
    public float? ValueFloat { get; set; }
}