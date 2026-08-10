using SQLite;

namespace SamwiseBlazor.DatabaseModels;

[Table("SensorDevices")]
public class SensorDevice
{
    [PrimaryKey]
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? IPAddress { get; set; } = string.Empty;
    public string? Location { get; set; } = string.Empty;
}