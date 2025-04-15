namespace Airport.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Airport.Data.Context.AirportDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Airport.Data.Context.AirportDbContext context)
        {
            context.Flights.AddOrUpdate(
                f => f.Id,
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(2),
                    ArrivalDate = DateTime.Now.AddHours(5),
                    FlightFrom = "New York",
                    FlightTo = "Los Angeles"
                },
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(3),
                    ArrivalDate = DateTime.Now.AddHours(6),
                    FlightFrom = "Chicago",
                    FlightTo = "Miami"
                },
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(4),
                    ArrivalDate = DateTime.Now.AddHours(7),
                    FlightFrom = "Seattle",
                    FlightTo = "San Francisco"
                },
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(5),
                    ArrivalDate = DateTime.Now.AddHours(8),
                    FlightFrom = "Boston",
                    FlightTo = "Washington"
                },
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(6),
                    ArrivalDate = DateTime.Now.AddHours(9),
                    FlightFrom = "Denver",
                    FlightTo = "Dallas"
                },
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(7),
                    ArrivalDate = DateTime.Now.AddHours(10),
                    FlightFrom = "Atlanta",
                    FlightTo = "Orlando"
                },
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(8),
                    ArrivalDate = DateTime.Now.AddHours(11),
                    FlightFrom = "Phoenix",
                    FlightTo = "Las Vegas"
                },
                new Models.Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(9),
                    ArrivalDate = DateTime.Now.AddHours(12),
                    FlightFrom = "Philadelphia",
                    FlightTo = "Newark"
                }
            );
        }
    }
}
