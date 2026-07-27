using SamwiseBlazor.DatabaseModels;
using SQLite;

namespace SamwiseBlazor.Services;

public class SqliteDatabase
{
    private readonly string _databasePath;

    public SqliteDatabase(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        var fileName = configuration["DatabaseFile"] ?? "Samwise.db";
        _databasePath = Path.IsPathRooted(fileName)
            ? fileName
            : Path.Combine(hostEnvironment.ContentRootPath, fileName);

        using var connection = GetConnection();
        connection.CreateTable<SensorData>();
        

        SeedDefaults(connection);
    }

    public SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_databasePath);
    }

    /// <summary>
    /// Seeds the database with default values.
    /// </summary>
    /// <param name="connection"></param>
    private static void SeedDefaults(SQLiteConnection connection)
    {
        //examples copied from another project
        // if (connection.Table<SystemSettings>().Count() == 0)
        // {
        //     connection.InsertAll(new[]
        //     {
        //         new SystemSettings { Key = "SettingName", Value = "0" }
        //     });
        // }
    }
}
