using Airport.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Ariport.Server.Repositories.Interfaces
{
    public interface IAirplaneTicketRepository : IGenericRepository<AirplaneTicket>
    {
        Task<List<AirplaneTicket>> PassengerTickets(Guid passngerId);
    }
}