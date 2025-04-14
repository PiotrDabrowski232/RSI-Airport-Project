using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Ariport.Server.Models
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