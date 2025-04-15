using Airport.Data.Enum;
using System.Runtime.Serialization;

namespace Airport.Data.Models
{
    public class AirplaneTicket
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid PassengerID { get; set; }

        public virtual Passenger Passenger { get; set; }

        [DataMember]
        public Guid FlightID { get; set; }

        public virtual Flight Flight { get; set; }

        [DataMember]
        public TicketStatus Status { get; set; }
    }   
}
