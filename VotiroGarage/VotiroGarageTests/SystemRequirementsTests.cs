using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VotiroGarage;

namespace VotiroGarageTests
{
    [TestClass]
    public class SystemRequirementsTests
    {
        // Requirement 1
        [TestMethod]
        public void CanPaintRed()
        {
            //Arrange
            var garage = new Garage();
            var garageLog = new GarageLog();
            garage.InitGarage(garageLog);
            var car = new Garage.ServicedCarTicket(1234567, "Tesla", Color.Black, 100);

            //Act
            garage.Serve(car, "Paint red");

            //Assert
            Assert.AreEqual(car.Color, Color.Red);
        }

        // Requirement 2
        [TestMethod]
        public void CanPaintBlue()
        {
            //Arrange
            var garage = new Garage();
            var garageLog = new GarageLog();
            garage.InitGarage(garageLog);
            var car = new Garage.ServicedCarTicket(1234567, "Tesla", Color.Red, 100);

            //Act
            garage.Serve(car, "Paint blue");

            //Assert
            Assert.AreEqual(Color.Blue, car.Color);
        }

        // Requirement 3
        [TestMethod]
        public void CanConvertNameToLowerCase()
        {
            //Arrange
            var garage = new Garage();
            var garageLog = new GarageLog();
            garage.InitGarage(garageLog);
            var car = new Garage.ServicedCarTicket(1234567, "BMW", Color.Green, 50);

            //Act
            garage.Serve(car, "Convert name to lower case");

            //Assert
            Assert.AreEqual("bmw", car.Name);
        }

        // Requirement 4
        [TestMethod]
        public void CanRefuel()
        {
            //Arrange
            var garage = new Garage();
            var garageLog = new GarageLog();
            garage.InitGarage(garageLog);
            var car = new Garage.ServicedCarTicket(7349158, "Subaru", Color.Blue, 3);

            //Act
            garage.Serve(car, "Refuel");

            //Assert
            Assert.AreEqual(100, car.FuelTank);
        }

        // Requirement 5
        [TestMethod]
        public void CanServeInASequence()
        {
            //Arrange
            var garage = new Garage();
            var garageLog = new GarageLog();
            garage.InitGarage(garageLog);
            var car = new Garage.ServicedCarTicket(7349158, "Subaru", Color.Blue, 3);

            //Act
            garage.Serve(car, "Paint blue");
            garage.Serve(car, "Convert name to lower case");
            garage.Serve(car, "Refuel");

            //Assert
            Assert.AreEqual(Color.Blue, car.Color);
            Assert.AreEqual("subaru", car.Name);
            Assert.AreEqual(100, car.FuelTank);
        }

        #region Requirement 6
        class MockGarageLog : IGarageLog
        {
            public string LogContents { get; private set; }
            public void Log(string message)
            {
                LogContents += message;
            }
        }

        [TestMethod]
        public void EachSuccessfullOperationIsLogged()
        {
            //Arrange
            var garage = new Garage();
            var mockGarageLog = new MockGarageLog();
            garage.InitGarage(mockGarageLog);
            var testCar = new Garage.ServicedCarTicket(1, "ONE NAME", Color.White, 10);
            garage.AddNewServiceOperation("Successfull operation",
                                          car => { },
                                          "serving car named {0}, colored {1} and with {2}% fuel");

            //Act
            garage.Serve(testCar, "Successfull operation");

            //Assert
            StringAssert.StartsWith(mockGarageLog.LogContents,
                                    "1: Started serving car named ONE NAME, colored white and with 10% fuel.",
                                    "Operation beginning not logged properly");
            StringAssert.EndsWith(mockGarageLog.LogContents,
                                  "1: Completed serving car named ONE NAME, colored white and with 10% fuel.",
                                  "Operation completion not logged properly");
        }

        class TestException : Exception
        {
            public TestException(string message) : base(message) { }
        }

        [TestMethod]
        public void EachFailedOperationIsLogged()
        {
            //Arrange
            var garage = new Garage();
            var mockGarageLog = new MockGarageLog();
            garage.InitGarage(mockGarageLog);
            var testCar = new Garage.ServicedCarTicket(13, "another name", Color.Black, 0);
            garage.AddNewServiceOperation("Failing operation",
                                          car => { throw new TestException("Staaam"); },
                                          "serving car named {0}, colored {1} and with {2}% fuel");

            //Act
            try
            {
                garage.Serve(testCar, "Failing operation");
                Assert.Fail("Failing operation did not threw exception");
            }
            catch (TestException)
            { }

            //Assert
            StringAssert.StartsWith(mockGarageLog.LogContents,
                                    "13: Started serving car named another name, colored black and with 0% fuel.",
                                    "Operation beginning not logged properly");
            StringAssert.EndsWith(mockGarageLog.LogContents,
                                  "13: Failed serving car named another name, colored black and with 0% fuel. Problem: Staaam.",
                                  "Operation failure not logged properly");
        }
        #endregion
    }
}
