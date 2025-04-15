using Airport.Data.Models;
using System.Data.Entity.ModelConfiguration;

namespace Airport.Data.ModelsConfig
{
    public class FlightConfiguration : EntityTypeConfiguration<Flight>
    {
        public FlightConfiguration()
        {
            ToTable("Flights");

            HasKey(f => f.Id);

            HasMany(f => f.AirplaneTickets)
                .WithRequired(at => at.Flight)
                .HasForeignKey(at => at.FlightID);
        }
    }


}
