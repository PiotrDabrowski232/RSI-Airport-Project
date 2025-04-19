using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Ariport.Server.Services;
using Ariport.Server.Services.Interfaces;

namespace Ariport.Server
{
    public class ServiceHost
    {
        public static void Main()
        {
            Uri baseAddress = new Uri("http://localhost:8080/Airport/");

            var binding = new WSHttpBinding(SecurityMode.None)
            {
                MessageEncoding = WSMessageEncoding.Mtom,
                MaxReceivedMessageSize = 10485760
            };

            using (var serviceHost = new System.ServiceModel.ServiceHost(typeof(AirportService), baseAddress))
            {
                serviceHost.AddServiceEndpoint(typeof(IFlightService), binding, "FlightService");
                serviceHost.AddServiceEndpoint(typeof(IPassengerService), binding, "PassengerService");
                serviceHost.AddServiceEndpoint(typeof(IAirplaneTicketService), binding, "TicketService");

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 }
                };
                serviceHost.Description.Behaviors.Add(smb);

                serviceHost.AddServiceEndpoint(
                    typeof(IMetadataExchange),
                    MetadataExchangeBindings.CreateMexHttpBinding(),
                    "mex");

                serviceHost.Open();

                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <Enter> to stop the service.");

                Console.ReadLine();

                serviceHost.Close();
            }
        }
    }
}
