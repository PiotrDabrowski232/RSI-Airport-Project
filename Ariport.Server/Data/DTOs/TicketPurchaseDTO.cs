using System;
using System.Runtime.Serialization;

namespace Airport.Server.DTOs
{
    [DataContract]
    public class TicketPurchaseDTO
    {
        [DataMember]
        public Guid FlightId { get; set; }

        [DataMember]
        public string PassengerName { get; set; }

        [DataMember]
        public string PassengerSurname { get; set; }

        [DataMember]
        public string PassengerPesel { get; set; }
    }
}
