using Ariport.Server.Data.DTOs;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Ariport.Server.Services.Interfaces
{
    [ServiceContract]
    public interface IAirplaneTicketService
    {

        [OperationContract]
        Task<Guid> PurchaseTicketAsync(Guid flightId, Guid passengerId);


        [OperationContract]
        Task<List<AirplaneTicketDto>> GetPassengerTickets(Guid passengerId);
    }
}
