using Airport.Server.Enum;
using System;

namespace Airport.Server.Models
{
    public class AirplaneTicket
    {
        public Guid Id { get; set; }
        public Guid PassengerID { get; set; }
        public virtual Passenger Passenger { get; set; }
        public Guid FlightID { get; set; }
        public virtual Flight Flight { get; set; }
        public TicketStatus Status { get; set; }
    }
}
