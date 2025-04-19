using Airport.Server.Context;
using Airport.Server.Models;
using Ariport.Server.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Data.Entity;

namespace Ariport.Server.Repositories
{
    public class AirplaneTicketRepository : GenericRepository<AirplaneTicket>, IGenericRepository<AirplaneTicket>, IAirplaneTicketRepository
    {
        public AirplaneTicketRepository(AirportDbContext context) : base(context)
        {
        }

        public async Task<List<AirplaneTicket>> PassengerTickets(Guid passngerId)
        {
            return await _context.AirplaneTickets
                .Where(x => x.PassengerID == passngerId)
                .Select(x => new AirplaneTicket
                {
                    Id = x.Id,
                    Passenger = x.Passenger,
                    Flight = x.Flight
                }).ToListAsync();
        }
        public async Task<AirplaneTicket> GetTicketDetailsByIdAsync(Guid ticketId)
        {
            return await _context.AirplaneTickets
                .Include(x => x.Passenger)
                .Include(x => x.Flight)
                .FirstOrDefaultAsync(x => x.Id == ticketId);
        }

    }
}