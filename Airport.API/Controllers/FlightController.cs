using Airport.API.Auth;
using Ariport.Server.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _flightService.GetFlightsAsync();
            return Ok(result);
        }

        [HttpGet("Search")]
        public async Task<ActionResult> SearchFlights([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime? departureDate)
        {
            var result = await _flightService.SearchFlightsAsync(from, to, departureDate);
            return Ok(result);
        }
    }
}
