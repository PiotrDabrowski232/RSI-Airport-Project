using Airport.API.Auth;
using Airport.Server.DTOs;
using Ariport.Server.Data.DTOs;
using Ariport.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Airport.API.Controllers
{
    [Route("AirplaneTicket")]
    [ApiController]
    [BasicAuthFilter]
    public class AirplaneTicketController : ControllerBase
    {
        private readonly IAirplaneTicketService _airplaneTicketService;

        public AirplaneTicketController(IAirplaneTicketService airplaneTicketService)
        {
            _airplaneTicketService = airplaneTicketService;
        }

        [HttpPost("Purchase")]
        public async Task<ActionResult> PurchaseTicket([FromBody] TicketPurchaseDTO ticketPurchaseDto)
        {
            try
            {
                var result = await _airplaneTicketService.PurchaseTicketAsync(ticketPurchaseDto);

                var resource = new TicketResource
                {
                    TicketID = result,
                    Links = new List<Link>
                    {
                        new Link(Url.Action("GetTicketById", new { ticketId = result}), "self", "GET"),
                        new Link($"/Flight/{result}/pdf", "ticket-pdf", "GET")
                    }
                };

                return Ok(resource);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas zakupu biletu." });
            }
        }


        [HttpGet("Passenger/{passengerId}")]
        public async Task<ActionResult> GetPassengerTickets(Guid passengerId)
        {
            try
            {
                var result = await _airplaneTicketService.GetPassengerTickets(passengerId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania biletów pasażera." });
            }
        }

        [HttpGet("{ticketId}")]
        public async Task<ActionResult> GetTicketById(Guid ticketId)
        {
            try
            {
                var result = await _airplaneTicketService.GetTicketByIdAsync(ticketId);
                if (result == null)
                    return NotFound(new { message = "Bilet nie został znaleziony." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania biletu." });
            }
        }
    }
}
