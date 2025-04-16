
using System;
using System.Collections.Generic;

namespace Airport.Server.Models
{
    public class Flight
    {
        public Guid Id { get; set; }
        public string FlightFrom { get; set; }
        public string FlightTo { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public virtual ICollection<AirplaneTicket> AirplaneTickets { get;  set; }
    }
}
