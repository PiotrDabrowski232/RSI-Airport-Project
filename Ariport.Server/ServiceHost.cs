using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Ariport.Server.Handlers;
using Ariport.Server.Services;
using Ariport.Server.Services.Interfaces;

namespace Ariport.Server
{
    public class ServiceHost
    {
        public static void Main()
        {
            Uri baseAddress = new Uri("https://172.20.10.3:8443/Airport/");

            var binding = new WSHttpBinding(SecurityMode.Transport)
            {
                MessageEncoding = WSMessageEncoding.Mtom,
                MaxReceivedMessageSize = 10485760
            };
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            using (var serviceHost = new System.ServiceModel.ServiceHost(typeof(AirportService), baseAddress))
            {
                var flightEndpoint = serviceHost.AddServiceEndpoint(typeof(IFlightService), binding, "FlightService");
                var passengerEndpoint = serviceHost.AddServiceEndpoint(typeof(IPassengerService), binding, "PassengerService");
                var ticketEndpoint = serviceHost.AddServiceEndpoint(typeof(IAirplaneTicketService), binding, "TicketService");

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                {
                    HttpsGetEnabled = true,
                    HttpGetEnabled = false,
                    MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 }
                };
                serviceHost.Description.Behaviors.Add(smb);

                serviceHost.AddServiceEndpoint(
                    ServiceMetadataBehavior.MexContractName,
                    MetadataExchangeBindings.CreateMexHttpsBinding(),
                    "mex");

                var loggingBehavior = new SoapMessageLoggerBehavior();
                flightEndpoint.Behaviors.Add(loggingBehavior);
                passengerEndpoint.Behaviors.Add(loggingBehavior);
                ticketEndpoint.Behaviors.Add(loggingBehavior);

                serviceHost.Open();

                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <Enter> to stop the service.");

                Console.ReadLine();

                serviceHost.Close();
            }
        }
    }
}
