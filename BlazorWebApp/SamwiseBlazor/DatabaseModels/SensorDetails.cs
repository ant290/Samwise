using SQLite;

namespace SamwiseBlazor.DatabaseModels;

[Table("SensorDetails")]
public class SensorDetails
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed(Name = "IX_SensorDetails_DeviceSensor", Order = 1, Unique = true)]
    public int SensorDeviceId { get; set; }
    [Indexed(Name = "IX_SensorDetails_DeviceSensor", Order = 2, Unique = true)]
    public int SensorId { get; set; }
    public SensorType SensorType { get; set; }
    public string? Description { get; set; } = string.Empty;
    public int? AlertInt { get; set; }
    public float? AlertFloat { get; set; }
    public bool? AlertBool { get; set; }
    public string? AlertString { get; set; } = string.Empty;
}