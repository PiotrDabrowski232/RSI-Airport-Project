using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Airport.Server.Models;
using Ariport.Server.Data.DTOs;

namespace Ariport.Server.Services.Interfaces
{
    [ServiceContract]
    public interface IPassengerService
    {
        [OperationContract]
        Task<List<PassengerDTO>> GetPassengers();

        [OperationContract]
        Task<PassengerDTO> GetPassenger(Guid id);

        [OperationContract]
        Task<Guid> CreatePassenger(string name, string surname, string pesel);

        [OperationContract]
        Task<List<AirplaneTicket>> GetPassengerTickets(Guid passengerId);
    }
}
