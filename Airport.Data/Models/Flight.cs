using System.Runtime.Serialization;

namespace Airport.Data.Models
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
