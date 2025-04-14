using System.Runtime.Serialization;

namespace Airport.Data.Models
{
    public class AirplaneTicket
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string PassengerName { get; set; }

        [DataMember]
        public string PassengerSurname { get; set; }

        [DataMember]
        public Guid FlightID { get; set; }

        public virtual Flight Flight { get; set; }
    }
}
