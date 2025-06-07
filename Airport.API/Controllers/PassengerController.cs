using Ariport.Server.Data.DTOs;
using Ariport.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.API.Controllers
{
    [Route("Passenger")]
    [ApiController]
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
            var result = await _passengerService.GetPassengers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPassengerById(Guid id)
        {
            var result = await _passengerService.GetPassenger(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreatePassenger([FromBody] PassengerDTO passenger)
        {
            var result = await _passengerService.CreatePassenger(passenger.Name, passenger.Surname, passenger.Pesel);
            return Ok(result);
        }


    }
}
