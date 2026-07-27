using SamwiseBlazor.DatabaseModels;

namespace SamwiseBlazor.Services;

public interface ISensorDataService
{
    int AddSensorData(SensorData sensorData);
    IEnumerable<SensorData> GetRecent(int count);
}

public class SensorDataService : ISensorDataService
{
    private readonly SqliteDatabase _sqliteDatabase;

    public SensorDataService(SqliteDatabase sqliteDatabase)
    {
        _sqliteDatabase = sqliteDatabase;
    }

    public int AddSensorData(SensorData sensorData)
    {
        using var connection = _sqliteDatabase.GetConnection();
        connection.Insert(sensorData);
        return sensorData.Id;
    }

    public IEnumerable<SensorData> GetRecent(int count)
    {
        using var connection = _sqliteDatabase.GetConnection();
        return connection.Table<SensorData>()
                         .OrderByDescending(sd => sd.Timestamp)
                         .Take(count)
                         .ToList();
    }
}