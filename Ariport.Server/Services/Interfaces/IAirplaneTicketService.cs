using Airport.Server.DTOs;
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
        Task<Guid> PurchaseTicketAsync(TicketPurchaseDTO ticketPurchaseDto);

        [OperationContract]
        Task<List<AirplaneTicketDto>> GetPassengerTickets(Guid passengerId);

        [OperationContract]
        Task<AirplaneTicketDto> GetTicketByIdAsync(Guid ticketId);
    }

}
