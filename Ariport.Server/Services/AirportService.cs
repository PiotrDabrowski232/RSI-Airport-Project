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
using PdfSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Ariport.Server.Services
{
    public class AirportService : IFlightService, IPassengerService, IAirplaneTicketService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IAirplaneTicketRepository _airplaneTicketRepository;
        private static readonly string TemplatePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Templates", "BoughtTicket.html");
        public AirportService()
        {
            IronPdf.License.LicenseKey = "IRONSUITE.82675.STUDENT.PB.EDU.PL.12387-303CB909E6-DEBAPKB6VF4U33-7367W2HJ6WUD-FHE5JP7XSZM7-QQH6CMX7INFR-WHKK3OOHX26K-H4SIY2LUI37U-ALYXZP-TQ3QNO5DS3GPEA-DEPLOYMENT.TRIAL-XCXGU2.TRIAL.EXPIRES.28.MAY.2025";
            var context = new AirportDbContext();
            _flightRepository = new FlightRepository(context);
            _passengerRepository = new PassengerRepository(context);
            _airplaneTicketRepository = new AirplaneTicketRepository(context);
        }

        public AirportService(IFlightRepository flightRepository, IPassengerRepository passengerRepository, IAirplaneTicketRepository airplaneTicketRepository)
        {
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
            _airplaneTicketRepository = airplaneTicketRepository;
        }

        #region FlightService
        public async Task<List<FlightDTO>> GetFlightsAsync()
        {
            try
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Nie można pobrać lotów. Spróbuj ponownie później");
            }
        }

        public async Task<List<FlightDTO>> SearchFlightsAsync(string from, string to, DateTime? departureDate)
        {
            try
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
            catch (Exception ex)
            {
                throw new ArgumentException("Nie można pobrać lotów. Spróbuj ponownie później");
            }
        }

        public async Task<byte[]> GetTicketConfirmationPdfAsync(Guid ticketId)
        {
            try
            {
                if (!File.Exists(TemplatePath))
                    throw new FileNotFoundException("Plik szablonu HTML nie został znaleziony.", TemplatePath);

                var result = await GetTicketByIdAsync(ticketId);
                if (result == null)
                    throw new ArgumentException("Nie znaleziono biletu o podanym ID");

                string htmlTemplate = File.ReadAllText(TemplatePath);

                string htmlContent = htmlTemplate
                    .Replace("{{TicketID}}", result.Id.ToString())
                    .Replace("{{FirstName}}", result.Name)
                    .Replace("{{LastName}}", result.Surname)
                    .Replace("{{PESEL}}", result.Pesel)
                    .Replace("{{FlightFrom}}", result.FlightFrom)
                    .Replace("{{FlightTo}}", result.FlightTo)
                    .Replace("{{DepartureDate}}", result.DepartureDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{ArrivalDate}}", result.ArrivalDate.ToString("yyyy-MM-dd HH:mm"))
                    .Replace("{{Status}}", Enum.GetName(typeof(TicketStatus), result.Status));

                using (var document = PdfGenerator.GeneratePdf(htmlContent, PageSize.A4))
                {
                    using (var stream = new MemoryStream())
                    {
                        document.Save(stream, false);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Nie można wygenerować PDF. Spróbuj ponownie później");
            }
        }

        #endregion

        #region IPassengerService

        public async Task<List<PassengerDTO>> GetPassengers()
        {
            try
            {
                var resut = await _passengerRepository.GetAll();
                return resut.Select(x => new PassengerDTO
                {
                    Name = x.Name,
                    Surname = x.Surname,
                    Pesel = x.Pesel
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Nie można pobrać użytkowników. Spróbuj ponownie później");
            }
        }

        public async Task<PassengerDTO> GetPassenger(Guid id)
        {
            try
            {
                var result = await _passengerRepository.GetById(id);
                return new PassengerDTO
                {
                    Name = result.Name,
                    Surname = result.Surname,
                    Pesel = result.Pesel
                };

            }
            catch (Exception ex)
            {
                throw new ArgumentException("Podany użytkownik nie istnieje");
            }
        }

        public async Task<Guid> CreatePassenger(string name, string surname, string pesel)
        {
            try
            {

                return await _passengerRepository.Add(new Passenger
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Surname = surname,
                    Pesel = pesel
                });
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Nie można dodać użytkownika. Spróbuj ponownie później");
            }
        }
        #endregion

        #region AirplaneTicket

        public async Task<Guid> PurchaseTicketAsync(TicketPurchaseDTO ticketPurchaseDto)
        {
            try
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
            catch (Exception ex)
            {
                throw new ArgumentException("Nie można zakupić biletu. Spróbuj ponownie później");
            }
        }


        public async Task<List<AirplaneTicketDto>> GetPassengerTickets(Guid passengerId)
        {
            try
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
            catch (Exception ex)
            {
                throw new ArgumentException("Nie można pobrać biletów. Spróbuj ponownie później");
            }
        }

        public async Task<AirplaneTicketDto> GetTicketByIdAsync(Guid ticketId)
        {
            try
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
            catch (Exception ex)
            {
                throw new ArgumentException("Nie można pobrać biletu. Spróbuj ponownie później");
            }
        }

        #endregion
    }
}