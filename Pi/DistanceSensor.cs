using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

using Serilog;

namespace Pi
{
    
    internal static class DistanceSensor
    {
        
        /// <summary>
        /// Init GpioController to open/set the in/out pins of the Gpio
        /// board and get measurement in cm. 
        /// </summary>
        /// <param name="triggerPin">Pin number to send ultra sonic wave.</param>
        /// <param name="echoPin">Pin number to receive the reflection.</param>
        public static double Measure(int triggerPin, int echoPin)
        {
            using var gpio = new GpioController();
            gpio.OpenPin(triggerPin);
            gpio.OpenPin(echoPin);

            gpio.SetPinMode(triggerPin, PinMode.Output);
            gpio.SetPinMode(echoPin, PinMode.Input);

            gpio.Write(triggerPin, PinValue.Low);

            var manualResetEvent = new ManualResetEvent(initialState: false);
            manualResetEvent.WaitOne(500);
            var pulseLength = new Stopwatch();

            // send pulse
            gpio.Write(triggerPin, PinValue.High);
            manualResetEvent.WaitOne(TimeSpan.FromMilliseconds(0.01));
            gpio.Write(triggerPin, PinValue.Low);

            // receive pulse
            while (gpio.Read(echoPin) == PinValue.Low)
            {
                // no op
            }

            pulseLength.Start();


            while (gpio.Read(echoPin) == PinValue.High)
            {
                // no op
            }

            pulseLength.Stop();

            // calculating distance
            var timeBetween = pulseLength.Elapsed;
            var distance = timeBetween.TotalSeconds * 17000;
            Log.Debug($"Time elapsed: {timeBetween.ToString()}. Distance measured: {distance}cm.");

            return distance;
        }
        
    }
    
}