using System.Collections.Generic;

namespace Ariport.Server.Data.DTOs
{
    public class TicketResource
    {
        public System.Guid TicketID { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();
    }
}