using System;
using System.Collections.Generic;

namespace Airport.Server.Models
{
    public class Passenger
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Pesel { get; set; }
        public ICollection<AirplaneTicket> AirplaneTickets { get; set; }
    }
}
