using Airport.Data.Models;
using System.Data.Entity.ModelConfiguration;

namespace Airport.Data.ModelsConfig
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
