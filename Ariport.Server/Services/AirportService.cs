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
using IronPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ariport.Server.Services
{
    public class AirportService : IFlightService, IPassengerService, IAirplaneTicketService
    {

        private readonly IFlightRepository _flightRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IAirplaneTicketRepository _airplaneTicketRepository;
        private static readonly string TemplatePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Templates", "BoughtTicket.html");
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
                Id = f.Id,
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
            try
            {
                if (!File.Exists(TemplatePath))
                    throw new FileNotFoundException("Plik szablonu PDF nie został znaleziony.", TemplatePath);

                var result = await GetTicketByIdAsync(ticketId);

                string HTMLFile = File.ReadAllText(TemplatePath);

                string output = HTMLFile
                    .Replace("{{TicketID}}", result.Id.ToString())
                    .Replace("{{FirstName}}", result.Name)
                    .Replace("{{LastName}}", result.Surname)
                    .Replace("{{PESEL}}", result.Pesel)
                    .Replace("{{FlightFrom}}", result.FlightFrom)
                    .Replace("{{FlightTo}}", result.FlightTo)
                    .Replace("{{DepartureDate}}", result.DepartureDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{ArrivalDate}}", result.ArrivalDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{Status}}", Enum.GetName(typeof(TicketStatus), result.Status));

                var renderer = new ChromePdfRenderer();
                var pdfDoc = renderer.RenderHtmlAsPdf(output);
                byte[] pdfBytes = pdfDoc.BinaryData;

                //Później do usunięcia
                //Na potrzebe testów żeby sprawdzić wygenerowany pliczek
                //W razie testów odkomentowac i zmienić ścieżkę docelową zapisu pliku
                //File.WriteAllBytes("C:\\Users\\Piotr\\Desktop\\files\\PotwierdzenieBiletu.pdf", pdfBytes);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
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

        public async Task<Guid> PurchaseTicketAsync(TicketPurchaseDTO ticketPurchaseDto)
        {
            var existingPassenger = (await _passengerRepository.GetAll())
                .FirstOrDefault(p => p.Pesel == ticketPurchaseDto.PassengerPesel);

            if (existingPassenger == null)
            {
                existingPassenger = new Passenger
                {
                    Id = Guid.NewGuid(),
                    Name = ticketPurchaseDto.PassengerName,
                    Surname = ticketPurchaseDto.PassengerSurname,
                    Pesel = ticketPurchaseDto.PassengerPesel
                };

                await _passengerRepository.Add(existingPassenger);
            }

            var ticket = new AirplaneTicket
            {
                Id = Guid.NewGuid(),
                FlightID = ticketPurchaseDto.FlightId,
                PassengerID = existingPassenger.Id,
                Status = TicketStatus.Reserved
            };

            return await _airplaneTicketRepository.Add(ticket);
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

        public async Task<AirplaneTicketDto> GetTicketByIdAsync(Guid ticketId)
        {
            var ticket = await _airplaneTicketRepository.GetTicketDetailsByIdAsync(ticketId);

            if (ticket == null)
                return null;

            return new AirplaneTicketDto
            {
                Id = ticket.Id,
                Name = ticket.Passenger.Name,
                Surname = ticket.Passenger.Surname,
                Pesel = ticket.Passenger.Pesel,
                FlightFrom = ticket.Flight.FlightFrom,
                FlightTo = ticket.Flight.FlightTo,
                DepartureDate = ticket.Flight.DepartureDate,
                ArrivalDate = ticket.Flight.ArrivalDate,
                Status = ticket.Status
            };
        }

        #endregion
    }
}