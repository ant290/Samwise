namespace SamwiseBlazor.DatabaseModels;
/// <summary>
/// Each sensor type expects a different value type mapped like:
/// SoilMoisture -> ValueInt the resistance read by the sensor, ValueBool where 0 = dry, 1 = wet
/// Temperature -> ValueFloat the temperature in Celsius
/// Humidity -> ValueFloat the humidity percentage
/// </summary>
public enum SensorType
{
    None,
    SoilMoisture,
    Temperature,
    Humidity,
    Battery
}