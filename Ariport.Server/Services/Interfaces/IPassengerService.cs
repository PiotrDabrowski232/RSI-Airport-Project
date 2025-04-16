using System;
using System.Collections.Generic;
using System.ServiceModel;
using Airport.Server.Models;

namespace Ariport.Server.Services.Interfaces
{
    [ServiceContract]
    public interface IPassengerService
    {
        [OperationContract]
        List<Passenger> GetPassengers();

        [OperationContract]
        Passenger GetPassenger(Guid id);

        [OperationContract]
        Guid CreatePassenger(string name, string surname, string pesel);

        [OperationContract]
        List<AirplaneTicket> GetPassengerTickets(Guid passengerId);
    }
}
