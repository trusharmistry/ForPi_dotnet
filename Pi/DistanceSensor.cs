using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

using Serilog;

namespace Pi
{
    public class DistanceSensor : IDisposable
    {
        /// <summary>
        /// Ctor to init GpioController. It opens and set in/out pins of the Gpio board. 
        /// </summary>
        /// <param name="triggerPin">Pin number to send ultra sonic wave.</param>
        /// <param name="echoPin">Pin number to receive the reflection.</param>
        public DistanceSensor(int triggerPin, int echoPin)
        {
            Gpio = new GpioController();
            TriggerPin = triggerPin;
            EchoPin = echoPin;

            Gpio.OpenPin(TriggerPin);
            Gpio.OpenPin(EchoPin);

            Gpio.SetPinMode(TriggerPin, PinMode.Output);
            Gpio.SetPinMode(EchoPin, PinMode.Input);

            Gpio.Write(TriggerPin, PinValue.Low);
            Log.Debug("Init `Gpio`.");
        }
        

        /// <summary>
        /// Measure distance in cm.
        /// </summary>
        public double Measure()
        {
            var manualResetEvent = new ManualResetEvent(initialState: false);
            manualResetEvent.WaitOne(500);
            var pulseLength = new Stopwatch();

            // send pulse
            Gpio.Write(TriggerPin, PinValue.High);
            manualResetEvent.WaitOne(TimeSpan.FromMilliseconds(0.01));
            Gpio.Write(TriggerPin, PinValue.Low);

            // receive pulse
            while (Gpio.Read(EchoPin) == PinValue.Low)
            {
                // no op
            }

            pulseLength.Start();


            while (Gpio.Read(EchoPin) == PinValue.High)
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
        
        
        /// <inheritdoc/>
        public void Dispose()
        {
            Gpio?.Dispose();
            Log.Debug("Disposed the `Gpio` instance.");
        }

        
        private GpioController Gpio { get; }
        private int TriggerPin { get; }
        private int EchoPin { get; }
        
    }
}