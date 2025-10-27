using LogicAppFunctions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LogicAppFunctionsTests
{
    [TestClass]
    public class LogicAppFunctionsTests
    {
        [TestMethod]
        public void LogicAppFunctions_Run_GreenPath()
        {
            //Arrange

            var mockLogger = new Mock<ILogger<LogicAppFunctions.LogicAppFunctions>>();

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory
                .Setup(f => f.CreateLogger(typeof(LogicAppFunctions.LogicAppFunctions).FullName!))
                .Returns(mockLogger.Object);
            
            
            var f = new LogicAppFunctions.LogicAppFunctions(mockLoggerFactory.Object);
            var expectedWeather = new LogicAppFunctions.LogicAppFunctions.Weather
            {
                ZipCode = 12345,
                CurrentWeather = "The current weather is 20 Celsius",
                DayLow = "The low for the day is 10 Celsius",
                DayHigh = "The high for the day is 30 Celsius"
            };

            // Act
            var result = f.Run(expectedWeather.ZipCode, "Celsius").Result;

            
            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.AreEqual(expectedWeather.ZipCode, result.ZipCode, "ZipCode should match.");
            Assert.IsTrue(result.CurrentWeather.Contains("Celsius"), "CurrentWeather should contain 'Celsius'.");
            Assert.IsTrue(result.DayLow.Contains("Celsius"), "DayLow should contain 'Celsius'.");
            Assert.IsTrue(result.DayHigh.Contains("Celsius"), "DayHigh should contain 'Celsius'.");
        }
    }
}
