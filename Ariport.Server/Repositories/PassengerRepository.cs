using Airport.Server.Context;
using Airport.Server.Models;
using Ariport.Server.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Ariport.Server.Repositories
{
    public class PassengerRepository : GenericRepository<Passenger>, IGenericRepository<Passenger>, IPassengerRepository
    {
        public PassengerRepository(AirportDbContext context) : base(context)
        {
        }

        public async Task<List<AirplaneTicket>> PassengerTickets(Guid passngerId)
        {
            return await _context.AirplaneTickets.Where(x => x.PassengerID == passngerId).ToListAsync();
        }
    }
}