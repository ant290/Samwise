using SQLite;

namespace SamwiseBlazor.DatabaseModels;

[Table("SensorData")]
public class SensorData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int SourceDeviceId { get; set; }
}
