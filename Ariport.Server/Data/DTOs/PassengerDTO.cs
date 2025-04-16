using System.Runtime.Serialization;

namespace Ariport.Server.Data.DTOs
{
    public class PassengerDTO
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string Pesel { get; set; }
    }
}