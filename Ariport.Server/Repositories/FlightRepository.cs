using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Airport.Server.Context;
using Airport.Server.Models;
using Airport.Server.Repositories.Interfaces;
using Ariport.Server.Repositories.Interfaces;
using Ariport.Server.Repositories;

namespace Airport.Server.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IGenericRepository<Flight>, IFlightRepository
    {
        public FlightRepository(AirportDbContext context) : base(context)
        {
        }
        public async Task<List<Flight>> SearchAsync(string from, string to, DateTime? departureDate)
        {
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrEmpty(from))
                query = query.Where(f => f.FlightFrom.Contains(from));

            if (!string.IsNullOrEmpty(to))
                query = query.Where(f => f.FlightTo.Contains(to));

            if (departureDate.HasValue)
            {
                var date = departureDate.Value.Date;
                query = query.Where(f => DbFunctions.TruncateTime(f.DepartureDate) == date);
            }

            return await query.ToListAsync();
        }

    }
}
