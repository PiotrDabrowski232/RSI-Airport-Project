using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Runtime.Serialization;

namespace Ariport.Server.Handlers
{
    [DataContract]
    public class CustomFault
    {
        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    public class GlobalErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            Console.WriteLine("Wystąpił wyjątek: " + error);
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            CustomFault customFault;

            if (error is ArgumentException)
            {
                customFault = new CustomFault
                {
                    ErrorCode = "ARGUMENT_ERROR",
                    ErrorMessage = error.Message
                };
            }
            else if (error is InvalidOperationException)
            {
                customFault = new CustomFault
                {
                    ErrorCode = "INVALID_OPERATION",
                    ErrorMessage = error.Message
                };
            }
            else
            {
                customFault = new CustomFault
                {
                    ErrorCode = "SERVER_ERROR",
                    ErrorMessage = "Wystąpił wewnętrzny błąd serwera."
                };
            }

            var faultException = new FaultException<CustomFault>(customFault, new FaultReason(customFault.ErrorMessage));

            var faultMessage = Message.CreateMessage(version, faultException.CreateMessageFault(), faultException.Action);
            fault = faultMessage;
        }
    }
}
