using Airport.Server.Enum;
using System;
using System.Runtime.Serialization;

namespace Ariport.Server.Data.DTOs
{
    public class AirplaneTicketDto
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public string Pesel { get; set; }
        [DataMember]
        public string FlightFrom { get; set; }
        [DataMember]
        public string FlightTo { get; set; }
        [DataMember]
        public DateTime DepartureDate { get; set; }
        [DataMember]
        public DateTime ArrivalDate { get; set; }
        [DataMember]
        public TicketStatus Status { get; set; }
    }
}