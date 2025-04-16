using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Airport.Server.Context;
using Airport.Server.DTOs;
using Airport.Server.Models;
using Airport.Server.Repositories;
using Airport.Server.Repositories.Interfaces;
using Ariport.Server.Services.Interfaces;

namespace Ariport.Server.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;

        public FlightService()
        {
            var context = new AirportDbContext();
            _flightRepository = new FlightRepository(context);
        }

        public FlightService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        public async Task<List<FlightDTO>> GetFlightsAsync()
        {
            var flights = await _flightRepository.GetAllAsync();
            return flights.Select(f => new FlightDTO
            {
                FlightFrom = f.FlightFrom,
                FlightTo = f.FlightTo,
                DepartureDate = f.DepartureDate,
                ArrivalDate = f.ArrivalDate
            }).ToList();
        }

        public async Task<List<FlightDTO>> SearchFlightsAsync(string from, string to, DateTime? departureDate)
        {
            var flights = await _flightRepository.SearchAsync(from, to, departureDate);
            return flights.Select(f => new FlightDTO
            {
                FlightFrom = f.FlightFrom,
                FlightTo = f.FlightTo,
                DepartureDate = f.DepartureDate,
                ArrivalDate = f.ArrivalDate
            }).ToList();
        }

        public async Task<Guid> PurchaseTicketAsync(Guid flightId, Guid passengerId)
        {
            // Implementacja logiki zakupu biletu
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetTicketConfirmationPdfAsync(Guid ticketId)
        {
            // Implementacja generowania PDF
            throw new NotImplementedException();
        }
    }
}
