using Airport.Data.Models;
using Airport.Data.ModelsConfig;
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
        public DbSet<Passenger> Passengers { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FlightConfiguration());
            modelBuilder.Configurations.Add(new AirplaneTicketConfiguration());
            modelBuilder.Configurations.Add(new PassengerConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }

}
