using Airport.Server.Context;
using Airport.Server.Models;
using Ariport.Server.Repositories.Interfaces;

namespace Ariport.Server.Repositories
{
    public class PassengerRepository : GenericRepository<Passenger>, IGenericRepository<Passenger>, IPassengerRepository
    {
        public PassengerRepository(AirportDbContext context) : base(context)
        {
        }

    }
}