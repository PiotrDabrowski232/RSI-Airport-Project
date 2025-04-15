using Airport.Data.Enum;

namespace Airport.Data.Models
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
