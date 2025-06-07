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
        [Route("/GetAllPassengers")]
        public async Task<ActionResult> GetPassenger()
        {
            var result = await _passengerService.GetPassengers();
            return Ok(result);
        }


    }
}
