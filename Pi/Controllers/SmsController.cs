using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace Pi.Controllers
{
    public class SmsController : TwilioController
    {
        private IConfiguration Configuration { get; }

        public SmsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public TwiMLResult Index(SmsRequest incomingMessage)
        {
            var messagingResponse = new MessagingResponse();
            if (string.IsNullOrEmpty(incomingMessage.Body))
            {
                messagingResponse.Message("Just ask for `status`"); 
                return TwiML(messagingResponse);
            }

            if (!double.TryParse(Configuration.GetSection("Distance:Minimum").Value, out var minDistInCm))
            {
                minDistInCm = 30.0;
            }

            if (incomingMessage.Body.ToLower() == "status")
            {
                try
                {
                    var distMeasuredBySensor = new UcSensor( triggerPin:18, echoPin:24).GetDistance();
                    var res = distMeasuredBySensor < minDistInCm
                        ? $"Garage door is Open! Distance measured from Pi to Door: {distMeasuredBySensor}cm"
                        : $"Garage door is Closed! Distance measured from Pi to Door: {distMeasuredBySensor}cm";

                    messagingResponse.Message(res);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Pi.Controllers.SmsController.Index");
                    Console.WriteLine($"Pi.Controllers.SmsController.Index: {e.InnerException}");
                    messagingResponse.Message($"{e.Message.Substring(0, 160)}");
                }
            }
            else
            {
                messagingResponse.Message("Just ask for STATUS.");
            }

            return TwiML(messagingResponse);
        }
    }
}