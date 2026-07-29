using SamwiseBlazor.DatabaseModels;

namespace SamwiseBlazor.Services;

public interface ISensorDataService
{
    int AddSensorData(SensorData sensorData);
    int AddSensorReading(SensorReading sensorReading);
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

    public int AddSensorReading(SensorReading sensorReading)
    {
        using var connection = _sqliteDatabase.GetConnection();
        connection.Insert(sensorReading);
        return sensorReading.Id;
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