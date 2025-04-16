using Airport.Server.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ariport.Server.Repositories.Interfaces
{
    public interface IPassengerRepository : IGenericRepository<Passenger>
    {
        Task<List<AirplaneTicket>> PassengerTickets(Guid passngerId);
    }
}