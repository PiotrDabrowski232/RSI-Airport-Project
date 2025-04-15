using System.Runtime.Serialization;

namespace Airport.Data.Models
{
    public class Passenger
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string Pesel { get; set; }
    }
}
