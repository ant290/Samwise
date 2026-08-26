using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SamwiseBlazor.Controllers;
using SamwiseBlazor.DatabaseModels;
using SamwiseBlazor.Dtos;
using SamwiseBlazor.Services;

namespace SamwiseBlazor.Tests;

public class GardenSensorControllerTests
{
    [Fact]
    public void PostGardenSensorData_AddsNewDevice()
    {
        var service = new Mock<ISensorDataService>();
        service.Setup(dataService => dataService.GetSensorDevice(7)).Returns((SensorDevice)null!);
        service.Setup(dataService => dataService.AddSensorData(It.IsAny<SensorData>())).Returns(1);
        var controller = CreateController(service.Object);

        var result = controller.PostGardenSensorData(new GardenSensorData
        {
            DeviceId = 7
        });

        Assert.IsType<OkObjectResult>(result);
        service.Verify(dataService => dataService.AddSensorDevice(It.Is<SensorDevice>(device =>
            device.Id == 7 && device.Name == "Device 7" && device.Location == "Unknown")), Times.Once);
        service.Verify(dataService => dataService.AddSensorData(It.IsAny<SensorData>()), Times.Once);
    }

    [Fact]
    public void PostGardenSensorData_UpdatesExistingDeviceIpAddress()
    {
        var service = new Mock<ISensorDataService>();
        service.Setup(dataService => dataService.GetSensorDevice(7)).Returns(new SensorDevice
        {
            Id = 7,
            Name = "Raised bed",
            IPAddress = "192.0.2.1",
            Location = "Garden"
        });
        service.Setup(dataService => dataService.AddSensorData(It.IsAny<SensorData>())).Returns(1);
        var controller = CreateController(service.Object);

        var result = controller.PostGardenSensorData(new GardenSensorData
        {
            DeviceId = 7
        });

        Assert.IsType<OkObjectResult>(result);
        service.Verify(dataService => dataService.UpdateSensorDevice(It.Is<SensorDevice>(device =>
            device.Id == 7 && device.IPAddress == "127.0.0.1")), Times.Once);
    }

    [Fact]
    public void PostGardenSensorData_AddsEachSensorReading()
    {
        var service = new Mock<ISensorDataService>();
        service.Setup(dataService => dataService.GetSensorDevice(7)).Returns((SensorDevice)null!);
        service.Setup(dataService => dataService.AddSensorData(It.IsAny<SensorData>())).Returns(1);
        var controller = CreateController(service.Object);

        var result = controller.PostGardenSensorData(new GardenSensorData
        {
            DeviceId = 7,
            SensorReadings =
            [
                new GardenSensorReading
                {
                    SensorId = 2,
                    SensorType = SensorType.Temperature,
                    ValueFloat = 21.5f
                },
                new GardenSensorReading
                {
                    SensorId = 3,
                    SensorType = SensorType.SoilMoisture,
                    ValueInt = 480,
                    ValueBool = false
                }
            ]
        });

        Assert.IsType<OkObjectResult>(result);
        service.Verify(dataService => dataService.AddSensorReading(It.Is<SensorReading>(reading =>
            reading.SensorId == 2 && reading.SensorType == SensorType.Temperature && reading.ValueFloat == 21.5f)), Times.Once);
        service.Verify(dataService => dataService.AddSensorReading(It.Is<SensorReading>(reading =>
            reading.SensorId == 3 && reading.SensorType == SensorType.SoilMoisture &&
            reading.ValueInt == 480 && reading.ValueBool == false)), Times.Once);
    }

    [Fact]
    public void PostGardenSensorData_AddsSensorDetailsForNewSensor()
    {
        var service = new Mock<ISensorDataService>();
        service.Setup(dataService => dataService.GetSensorDevice(7)).Returns((SensorDevice)null!);
        service.Setup(dataService => dataService.GetSensorDetails(7, 2)).Returns((SensorDetails?)null);
        service.Setup(dataService => dataService.AddSensorData(It.IsAny<SensorData>())).Returns(1);
        var controller = CreateController(service.Object);

        var result = controller.PostGardenSensorData(new GardenSensorData
        {
            DeviceId = 7,
            SensorReadings =
            [
                new GardenSensorReading
                {
                    SensorId = 2,
                    SensorType = SensorType.Temperature,
                    ValueFloat = 21.5f
                }
            ]
        });

        Assert.IsType<OkObjectResult>(result);
        service.Verify(dataService => dataService.AddSensorDetails(It.Is<SensorDetails>(details =>
            details.SensorDeviceId == 7 && details.SensorId == 2 &&
            details.SensorType == SensorType.Temperature)), Times.Once);
    }

    [Fact]
    public void PostGardenSensorData_UpdatesSensorTypeForExistingSensor()
    {
        var service = new Mock<ISensorDataService>();
        var existingDetails = new SensorDetails
        {
            Id = 1,
            SensorDeviceId = 7,
            SensorId = 2,
            SensorType = SensorType.Temperature,
            Description = "Bed sensor",
            AlertFloat = 35
        };
        service.Setup(dataService => dataService.GetSensorDevice(7)).Returns((SensorDevice)null!);
        service.Setup(dataService => dataService.GetSensorDetails(7, 2)).Returns(existingDetails);
        service.Setup(dataService => dataService.AddSensorData(It.IsAny<SensorData>())).Returns(1);
        var controller = CreateController(service.Object);

        var result = controller.PostGardenSensorData(new GardenSensorData
        {
            DeviceId = 7,
            SensorReadings =
            [
                new GardenSensorReading
                {
                    SensorId = 2,
                    SensorType = SensorType.Humidity,
                    ValueFloat = 60
                }
            ]
        });

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(SensorType.Humidity, existingDetails.SensorType);
        service.Verify(dataService => dataService.UpdateSensorDetails(existingDetails), Times.Once);
    }

    private static GardenSensorController CreateController(ISensorDataService service)
    {
        return new GardenSensorController(
            NullLogger<GardenSensorController>.Instance,
            service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Connection =
                    {
                        RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1")
                    }
                }
            }
        };
    }

}