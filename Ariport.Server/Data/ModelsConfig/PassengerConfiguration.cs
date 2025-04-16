using Airport.Server.Models;
using System.Data.Entity.ModelConfiguration;

namespace Airport.Server.ModelsConfig
{
    public class PassengerConfiguration : EntityTypeConfiguration<Passenger>
    {
        public PassengerConfiguration()
        {
            ToTable("Passengers");

            HasKey(p => p.Id);

            HasMany(p => p.AirplaneTickets)
                .WithRequired(at => at.Passenger)
                .HasForeignKey(at => at.PassengerID);


        }
    }
}
