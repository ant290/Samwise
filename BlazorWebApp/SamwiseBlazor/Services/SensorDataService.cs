using SamwiseBlazor.DatabaseModels;

namespace SamwiseBlazor.Services;

public interface ISensorDataService
{
    int AddSensorData(SensorData sensorData);
    int AddSensorReading(SensorReading sensorReading);
    IEnumerable<SensorData> GetRecent(int count);
    List<FlatSensorReading> GetReadings(int daysBack, SensorType? sensorType = null, int? sourceDeviceId = null);
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

    public List<FlatSensorReading> GetReadings(int daysBack, SensorType? sensorType = null, int? sourceDeviceId = null)
    {
        string sensorTypeWhere = sensorType == null ? "" : $"AND sr.sensorType = {sensorType.Value} ";
        string sourceDeviceWhere = sourceDeviceId == null ? "" : $"AND sd.sourceDeviceId = {sourceDeviceId} ";

        using var connection = _sqliteDatabase.GetConnection();
        string query = "SELECT sd.sourceDeviceId, sd.timestamp, sr.sensorId, sr.sensorType, sr.valueInt, sr.valueBool, sr.valueString " +
                        "FROM SensorData sd "+
                        "JOIN SensorReadings sr ON sd.Id = sr.sensorDataId " +
                        $"WHERE CAST((sd.timestamp - 621355968000000000) / 10000000 AS INTEGER) >= strftime('%s', 'now', '-{daysBack} days')" +
                        sensorTypeWhere +
                        sourceDeviceWhere;
        //string daysBackParam = $"'-{daysBack} days'";
        return connection.Query<FlatSensorReading>(query, daysBack);
    }
}