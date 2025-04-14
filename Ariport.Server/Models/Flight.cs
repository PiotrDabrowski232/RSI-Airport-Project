using System;
using System.Runtime.Serialization;

namespace Ariport.Server.Models
{
    public class Flight
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string FlightFrom { get; set; }

        [DataMember]
        public string FlightTo { get; set; }

        [DataMember]
        public DateTime DepartureDate { get; set; }

        [DataMember]
        public DateTime ArrivalDate { get; set; }
    }
}