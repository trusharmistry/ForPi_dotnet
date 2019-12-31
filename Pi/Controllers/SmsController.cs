using System;

using Microsoft.Extensions.Configuration;

using Serilog;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace Pi.Controllers
{
    public class SmsController : TwilioController
    {
        
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="configuration"></param>
        public SmsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        /// <summary>
        /// Default api to intercept the twilio sms. https://url/sms 
        /// </summary>
        public TwiMLResult Index(SmsRequest incomingMessage)
        {
            Log.Debug($"Sms request message: {incomingMessage}"); 
            var messagingResponse = new MessagingResponse();
            if (string.IsNullOrEmpty(incomingMessage.Body))
            {
                messagingResponse.Message("Just ask for `status`"); 
                return TwiML(messagingResponse);
            }

            if (!double.TryParse(Configuration.GetSection("Distance:Minimum").Value, out var minDistInCm))
            {
                Log.Debug("Unable to parse minimum distance in configuration. Using 30cm as default.");
                minDistInCm = 30.0;
            }

            if (incomingMessage.Body.ToLower() == "status")
            {
                try
                {
                    var distMeasuredBySensor = DistanceSensor.Measure(triggerPin: 18, echoPin: 24);
                    var res = distMeasuredBySensor < minDistInCm
                        ? $"Garage door is Open! Distance measured from Pi to Door: {distMeasuredBySensor}cm"
                        : $"Garage door is Closed! Distance measured from Pi to Door: {distMeasuredBySensor}cm";

                    messagingResponse.Message(res);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Pi.Controllers.SmsController.Index");
                    messagingResponse.Message($"{e.Message.Substring(0, 160)}");
                }
            }
            else
            {
                messagingResponse.Message("Just ask: STATUS.");
            }

            return TwiML(messagingResponse);
        }
        
        
        private IConfiguration Configuration { get; }
        
    }
}