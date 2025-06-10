using Airport.Server.Models;
using Airport.Server.ModelsConfig;
using System.Data.Entity;

namespace Airport.Server.Context
{
    public class AirportDbContext : DbContext
    {
        public AirportDbContext(string connectionString)
            : base(connectionString)
        {
        }
        public AirportDbContext(): base("AirportServiceDbContext")
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
