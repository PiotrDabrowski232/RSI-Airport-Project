using Airport.Data.Models;
using System.Data.Entity;

namespace Airport.Data.Context
{
    public class AirportDbContext : DbContext
    {
         public AirportDbContext(): base("name=AirportServiceDbContext")
        {
        }

        public DbSet<AirplaneTicket> AirplaneTickets { get; set; }
        public DbSet<Flight> Flights { get; set; }
    }

}
