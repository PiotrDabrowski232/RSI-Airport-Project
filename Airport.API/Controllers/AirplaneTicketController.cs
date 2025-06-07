using Airport.API.Auth;
using Airport.Server.DTOs;
using Ariport.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _airplaneTicketService.PurchaseTicketAsync(ticketPurchaseDto);
            return Ok(result);
        }

        [HttpGet("Passenger/{passengerId}")]
        public async Task<ActionResult> GetPassengerTickets(Guid passengerId)
        {
            var result = await _airplaneTicketService.GetPassengerTickets(passengerId);
            return Ok(result);
        }

        [HttpGet("{ticketId}")]
        public async Task<ActionResult> GetTicketById(Guid ticketId)
        {
            var result = await _airplaneTicketService.GetTicketByIdAsync(ticketId);
            return Ok(result);
        }
    }
}
