using System.Data.Entity;

namespace Ariport.Server.DBContext
{
    public class AirportServiceDbContext : DbContext
    {
        public AirportServiceDbContext() : base("name=AirportServiceDbContext")
        {
        }
        public DbSet<Models.AirplaneTicket> AirplaneTickets { get; set; }
        public DbSet<Models.Flight> Flights { get; set; }
    }
}