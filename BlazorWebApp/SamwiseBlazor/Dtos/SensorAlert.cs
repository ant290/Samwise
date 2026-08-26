using SamwiseBlazor.DatabaseModels;

namespace SamwiseBlazor.Dtos;

public class SensorAlert
{
    public int SensorDeviceId { get; set; }
    public string? SensorDeviceName { get; set; }
    public int SensorId { get; set; }
    public SensorType SensorType { get; set; }
    public string? SensorDescription { get; set; }
    public int? AlertInt { get; set; }
    public float? AlertFloat { get; set; }
    public bool? AlertBool { get; set; }
    public string? AlertString { get; set; }
    public FlatSensorReading LastReading { get; set; } = new();
}