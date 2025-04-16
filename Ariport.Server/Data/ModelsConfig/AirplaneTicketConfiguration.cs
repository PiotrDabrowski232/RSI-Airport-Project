using Airport.Server.Models;
using System.Data.Entity.ModelConfiguration;

namespace Airport.Server.ModelsConfig
{
    public class AirplaneTicketConfiguration : EntityTypeConfiguration<AirplaneTicket>
    {
        public AirplaneTicketConfiguration()
        {
            ToTable("AirplaneTickets");

            HasKey(at => at.Id);


        }
    }
}
