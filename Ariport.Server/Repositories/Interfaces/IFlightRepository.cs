using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Airport.Server.Models;
using Ariport.Server.Repositories.Interfaces;

namespace Airport.Server.Repositories.Interfaces
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        Task<List<Flight>> SearchAsync(string from, string to, DateTime? departureDate);
    }
}
