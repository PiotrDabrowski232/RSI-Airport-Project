using System.Collections.Generic;
using System.ServiceModel;
using Airport.Server.Models;

namespace Ariport.Server.Services.Interfaces
{
    [ServiceContract]
    public interface IFlightService 
    {
        [OperationContract]
        List<Flight> GetFlights(); 
    }
}