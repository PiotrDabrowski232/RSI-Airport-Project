using Airport.API.Auth;
using Ariport.Server.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Airport.API.Controllers
{
    [Route("Flight")]
    [ApiController]
    [BasicAuthFilter]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult> GetAllFlights()
        {
            try
            {
                var result = await _flightService.GetFlightsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Wystąpił błąd podczas pobierania lotów." });
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult> SearchFlights([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime? departureDate)
        {
            try
            {
                var result = await _flightService.SearchFlightsAsync(from, to, departureDate);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas wyszukiwania lotów." });
            }
        }

        [HttpGet("{ticketId}/pdf")]
        public async Task<IActionResult> GetTicketPdf(Guid ticketId)
        {
            try
            {
                var pdfBytes = await _flightService.GetTicketConfirmationPdfAsync(ticketId);
                return File(pdfBytes, "application/pdf", $"ticket-{ticketId}.pdf");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas generowania PDF-a." });
            }
        }
    }
}
