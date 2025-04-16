using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Ariport.Server.Handlers
{
    public class SoapMessageLogger : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            Console.WriteLine("Received request: " + request.ToString());
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            Console.WriteLine("Sending response: " + reply.ToString());
        }
    }

    public class SoapMessageLoggerBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new SoapMessageLogger());
        }

        public void Validate(ServiceEndpoint endpoint) { }
    }
}
