namespace Ariport.Server.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Airport.Server.Context;
    using Airport.Server.Models; 

    internal sealed class Configuration : DbMigrationsConfiguration<Airport.Server.Context.AirportDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Airport.Server.Context.AirportDbContext context)
        {
            context.Flights.AddOrUpdate(
                f => f.Id,
                new Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(1),
                    ArrivalDate = DateTime.Now.AddHours(3),
                    FlightFrom = "JFK",
                    FlightTo = "LAX"
                },
                new Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(2),
                    ArrivalDate = DateTime.Now.AddHours(4),
                    FlightFrom = "LAX",
                    FlightTo = "ORD"
                },
                new Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(3),
                    ArrivalDate = DateTime.Now.AddHours(5),
                    FlightFrom = "ORD",
                    FlightTo = "DFW"
                },
                new     Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(4),
                    ArrivalDate = DateTime.Now.AddHours(6),
                    FlightFrom = "DFW",
                    FlightTo = "MIA"
                },
                new Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(5),
                    ArrivalDate = DateTime.Now.AddHours(7),
                    FlightFrom = "MIA",
                    FlightTo = "SEA"
                },
                new Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(6),
                    ArrivalDate = DateTime.Now.AddHours(8),
                    FlightFrom = "SEA",
                    FlightTo = "BOS"
                },
                new Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(7),
                    ArrivalDate = DateTime.Now.AddHours(9),
                    FlightFrom = "BOS",
                    FlightTo = "SFO"
                },
                new Flight
                {
                    Id = Guid.NewGuid(),
                    DepartureDate = DateTime.Now.AddHours(8),
                    ArrivalDate = DateTime.Now.AddHours(10),
                    FlightFrom = "SFO",
                    FlightTo = "PHX"
                }
            );
        }
    }
}
