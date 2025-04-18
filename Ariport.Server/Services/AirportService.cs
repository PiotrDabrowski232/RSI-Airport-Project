using Airport.Server.Context;
using Airport.Server.DTOs;
using Airport.Server.Enum;
using Airport.Server.Models;
using Airport.Server.Repositories;
using Airport.Server.Repositories.Interfaces;
using Ariport.Server.Data.DTOs;
using Ariport.Server.Repositories;
using Ariport.Server.Repositories.Interfaces;
using Ariport.Server.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ariport.Server.Services
{
    public class AirportService : IFlightService, IPassengerService, IAirplaneTicketService
    {

        private readonly IFlightRepository _flightRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IAirplaneTicketRepository _airplaneTicketRepository;

        public AirportService()
        {
            var context = new AirportDbContext();
            _flightRepository = new FlightRepository(context);
            _passengerRepository = new PassengerRepository(context);
            _airplaneTicketRepository = new AirplaneTicketRepository(context);
        }

        public AirportService(IFlightRepository flightRepository, IPassengerRepository passengerRepository)
        {
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
        }

        #region FlightService
        public async Task<List<FlightDTO>> GetFlightsAsync()
        {
            var flights = await _flightRepository.GetAll();
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

        public async Task<byte[]> GetTicketConfirmationPdfAsync(Guid ticketId)
        {
            // Implementacja generowania PDF
            throw new NotImplementedException();
        }
        #endregion

        #region IPassengerService

        public async Task<List<PassengerDTO>> GetPassengers()
        {
            var resut = await _passengerRepository.GetAll();
            return resut.Select(x => new PassengerDTO
            {
                Name = x.Name,
                Surname = x.Surname,
                Pesel = x.Pesel
            }).ToList();
        }

        public async Task<PassengerDTO> GetPassenger(Guid id)
        {
            var result = await _passengerRepository.GetById(id);
            return new PassengerDTO
            {
                Name = result.Name,
                Surname = result.Surname,
                Pesel = result.Pesel
            };
        }

        public async Task<Guid> CreatePassenger(string name, string surname, string pesel)
        {
            return await _passengerRepository.Add(new Passenger
            {
                Id = Guid.NewGuid(),
                Name = name,
                Surname = surname,
                Pesel = pesel
            });
        }
        #endregion

        #region AirplaneTicket

        public async Task<Guid> PurchaseTicketAsync(Guid flightId, Guid passengerId)
        {
             return await _airplaneTicketRepository.Add(new AirplaneTicket
            {
                Id = Guid.NewGuid(),
                FlightID = flightId,
                PassengerID = passengerId,
                Status = TicketStatus.Purchased
            });
        }

        public async Task<List<AirplaneTicketDto>> GetPassengerTickets(Guid passengerId)
        {
            var result = await _airplaneTicketRepository.PassengerTickets(passengerId);

            return result.Select(x => new AirplaneTicketDto
            {
                Id = x.Id,
                Name = x.Passenger.Name,
                Surname = x.Passenger.Surname,
                Pesel = x.Passenger.Pesel,
                FlightFrom = x.Flight.FlightFrom,
                FlightTo = x.Flight.FlightTo,
                DepartureDate = x.Flight.DepartureDate,
                ArrivalDate = x.Flight.ArrivalDate,
                Status = x.Status
            }).ToList();
        }

        #endregion
    }
}