using Airport.API.Auth;
using Ariport.Server.Data.DTOs;
using Ariport.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Airport.API.Controllers
{
    [Route("Passenger")]
    [ApiController]
    [BasicAuthFilter]
    public class PassengerController : ControllerBase
    {
        private readonly IPassengerService _passengerService;

        public PassengerController(IPassengerService passengerService)
        {
            _passengerService = passengerService;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult> GetAllPassengers()
        {
            try
            {
                var result = await _passengerService.GetPassengers();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Wystąpił błąd podczas pobierania pasażerów." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPassengerById(Guid id)
        {
            try
            {
                var result = await _passengerService.GetPassenger(id);
                if (result == null)
                    return NotFound(new { message = "Pasażer nie został znaleziony." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania danych pasażera." });
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreatePassenger([FromBody] PassengerDTO passenger)
        {
            try
            {
                var result = await _passengerService.CreatePassenger(passenger.Name, passenger.Surname, passenger.Pesel);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas tworzenia pasażera." });
            }
        }
    }
}
