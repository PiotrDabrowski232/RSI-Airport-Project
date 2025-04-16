using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Airport.Server.DTOs;

namespace Ariport.Server.Services.Interfaces
{
    [ServiceContract]
    public interface IFlightService
    {
        [OperationContract]
        Task<List<FlightDTO>> GetFlightsAsync();

        [OperationContract]
        Task<List<FlightDTO>> SearchFlightsAsync(string from, string to, DateTime? departureDate);

        [OperationContract]
        Task<Guid> PurchaseTicketAsync(Guid flightId, Guid passengerId);

        [OperationContract]
        [ServiceKnownType(typeof(byte[]))]
        Task<byte[]> GetTicketConfirmationPdfAsync(Guid ticketId);
    }
}
