using System;
using System.Collections.Generic;
using System.Linq;

namespace VotiroGarage
{
    public class Garage
    {
        public class ServicedCarTicket : Car
        {
            //Stuff important for a garage like reference to car owner record, car maintenance history and other. For example:
            public int LicensePlate = 0;
            public string ImmobiliserCode = null;
            //etc.

            //Register new client
            public ServicedCarTicket(int id, string name, Color color, int fuel)
            {
                this.LicensePlate = id;
                this.Name = name;
                this.Color = color;
                this.FuelTank = fuel;
            }
        }

        public delegate void ServiceOperation(Car car);

        // Requirements 7 & 8
        public void InitGarage(IGarageLog garageLog)
        {
            this.garageLog = garageLog;
            availableOperationsList = new ServiceOperationsList();
            AddNewServiceOperation("Paint red",
                                   car => car.Color = Color.Red,
                                   "painting red on {1}");
            AddNewServiceOperation("Paint blue",
                                   car => car.Color = Color.Blue,
                                   "painting blue on {1}");
            AddNewServiceOperation("Convert name to lower case",
                                   car => car.Name = car.Name.ToLower(),
                                   "converting '{0}' name to lower case");
            AddNewServiceOperation("Refuel",
                                   car => car.FuelTank += 100 - car.FuelTank,
                                   "refueling from {2}");
        }

        // Requirement 9
        /// <summary>
        /// This can be used to add new operation without opening garage code (the last Requirement)
        /// </summary>
        /// <param name="serviceOperationName">Codename for the service operation</param>
        /// <param name="serviceOperation">Function on car executing the operation</param>
        /// <param name="logFormat">Log message format where {0} is car name, {1} is car color and {2} is car fuel level _before_ the operation</param>
        public void AddNewServiceOperation(string serviceOperationName, ServiceOperation serviceOperation, string logFormat)
        {
            availableOperationsList.Add(serviceOperationName, new ServiceOperationDetails(serviceOperation, logFormat));
        }

        /// <summary>
        /// Can be used to present available service operations to a user
        /// </summary>
        /// <returns></returns>
        public string[] GetAvailableServiceOperations()
        {
            return availableOperationsList.Keys.ToArray();
        }

        /// <summary>
        /// Perform a single servicefor a car
        /// </summary>
        /// <param name="car">Car ticket</param>
        /// <param name="serviceOperationName">Service operation codename</param>
        public void Serve(ServicedCarTicket car, string serviceOperationName)
        {
            var serviceOperationDetails = availableOperationsList[serviceOperationName];
            var logMessage = string.Format(serviceOperationDetails.LogFormat, car.Name, car.Color.ToString().ToLower(), car.FuelTank);
            var carId = car.LicensePlate;
            garageLog.Log(string.Format("{0}: Started {1}.", carId, logMessage));
            try
            {
                serviceOperationDetails.ServiceOperation(car);
                garageLog.Log(string.Format("{0}: Completed {1}.", carId, logMessage));
            }
            catch (Exception ex)
            {
                garageLog.Log(string.Format("{0}: Failed {1}. Problem: {2}.", carId, logMessage, ex.Message));
                throw;
            }
        }

        #region Private
        private class ServiceOperationDetails
        {
            public ServiceOperation ServiceOperation;
            public string LogFormat;
            public ServiceOperationDetails(ServiceOperation serviceOperation, string logFormat)
            {
                this.ServiceOperation = serviceOperation;
                this.LogFormat = logFormat;
            }
        }

        private class ServiceOperationsList : SortedDictionary<string, ServiceOperationDetails> { }

        private ServiceOperationsList availableOperationsList = null;
        private IGarageLog garageLog = null;
        #endregion
    }
}
