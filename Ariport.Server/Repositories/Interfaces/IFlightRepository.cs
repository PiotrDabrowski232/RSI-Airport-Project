using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Airport.Server.Models;

namespace Airport.Server.Repositories.Interfaces
{
    public interface IFlightRepository
    {
        Task<List<Flight>> GetAllAsync();
        Task<Flight> GetByIdAsync(Guid id);
        Task<List<Flight>> SearchAsync(string from, string to, DateTime? departureDate);
    }
}
