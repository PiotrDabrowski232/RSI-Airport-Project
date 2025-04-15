namespace Airport.Data.Models
{
    public class Flight
    {
        public Guid Id { get; set; }
        public string FlightFrom { get; set; }
        public string FlightTo { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
    }
}
