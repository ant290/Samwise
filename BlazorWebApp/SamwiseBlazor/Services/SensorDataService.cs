using SamwiseBlazor.DatabaseModels;

namespace SamwiseBlazor.Services;

public interface ISensorDataService
{
    SensorDevice GetSensorDevice(int deviceId);
    List<SensorDevice> GetSensorDevices();
    int AddSensorDevice(SensorDevice sensorDevice);
    bool UpdateSensorDevice(SensorDevice sensorDevice);
    SensorDetails? GetSensorDetails(int sensorDeviceId, int sensorId);
    List<SensorDetails> GetSensorDetails(int sensorDeviceId);
    List<FlatSensorReading> GetLatestReadings(int sensorDeviceId);
    int AddSensorDetails(SensorDetails sensorDetails);
    bool UpdateSensorDetails(SensorDetails sensorDetails);
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

    public SensorDevice GetSensorDevice(int deviceId)
    {
        using var connection = _sqliteDatabase.GetConnection();
        return connection.Table<SensorDevice>().FirstOrDefault(sd => sd.Id == deviceId);
    }

    public List<SensorDevice> GetSensorDevices()
    {
        using var connection = _sqliteDatabase.GetConnection();
        return connection.Table<SensorDevice>().OrderBy(sd => sd.Id).ToList();
    }

    public int AddSensorDevice(SensorDevice sensorDevice)
    {
        using var connection = _sqliteDatabase.GetConnection();
        connection.Insert(sensorDevice);
        return sensorDevice.Id;
    }

    public bool UpdateSensorDevice(SensorDevice sensorDevice)
    {
        using var connection = _sqliteDatabase.GetConnection();

        return connection.Update(sensorDevice) > 0;
    }

    public SensorDetails? GetSensorDetails(int sensorDeviceId, int sensorId)
    {
        using var connection = _sqliteDatabase.GetConnection();
        return connection.Table<SensorDetails>()
                         .FirstOrDefault(sd => sd.SensorDeviceId == sensorDeviceId && sd.SensorId == sensorId);
    }

    public List<SensorDetails> GetSensorDetails(int sensorDeviceId)
    {
        using var connection = _sqliteDatabase.GetConnection();
        return connection.Table<SensorDetails>()
                         .Where(sd => sd.SensorDeviceId == sensorDeviceId)
                         .OrderBy(sd => sd.SensorId)
                         .ToList();
    }

    public List<FlatSensorReading> GetLatestReadings(int sensorDeviceId)
    {
        using var connection = _sqliteDatabase.GetConnection();
        const string query = "SELECT sd.sourceDeviceId, sd.timestamp, " +
                             "sr.sensorId, sr.sensorType, sr.valueInt, " +
                             "sr.valueBool, sr.valueString, sr.valueFloat, sdv.name AS SensorDeviceName " +
                             "FROM SensorData sd " +
                             "JOIN SensorReadings sr ON sd.Id = sr.sensorDataId " +
                             "JOIN SensorDevices sdv ON sd.sourceDeviceId = sdv.Id " +
                             "WHERE sd.sourceDeviceId = ? " +
                             "ORDER BY sd.timestamp DESC";

        return connection.Query<FlatSensorReading>(query, sensorDeviceId)
                         .GroupBy(reading => reading.SensorId)
                         .Select(group => group.First())
                         .ToList();
    }

    public int AddSensorDetails(SensorDetails sensorDetails)
    {
        using var connection = _sqliteDatabase.GetConnection();
        connection.Insert(sensorDetails);
        return sensorDetails.Id;
    }

    public bool UpdateSensorDetails(SensorDetails sensorDetails)
    {
        using var connection = _sqliteDatabase.GetConnection();
        return connection.Update(sensorDetails) > 0;
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
        string sensorTypeWhere = sensorType == null ? "" : $"AND sr.sensorType = {(int?)sensorType} ";
        string sourceDeviceWhere = sourceDeviceId == null ? "" : $"AND sd.sourceDeviceId = {sourceDeviceId} ";

        using var connection = _sqliteDatabase.GetConnection();
        string query = "SELECT sd.sourceDeviceId, sd.timestamp, " +
                        "sr.sensorId, sr.sensorType, sr.valueInt, " +
                        "sr.valueBool, sr.valueString, sr.valueFloat, sdv.name AS SensorDeviceName " +
                        "FROM SensorData sd " +
                        "JOIN SensorReadings sr ON sd.Id = sr.sensorDataId " +
                        "JOIN SensorDevices sdv ON sd.sourceDeviceId = sdv.Id " +
                        $"WHERE CAST((sd.timestamp - 621355968000000000) / 10000000 AS INTEGER) >= strftime('%s', 'now', '-{daysBack} days') " +
                        sensorTypeWhere +
                        sourceDeviceWhere;
        var result = connection.Query<FlatSensorReading>(query);
        return result;
    }
}