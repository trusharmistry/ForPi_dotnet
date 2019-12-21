using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

using Serilog;

namespace Pi
{
    public class UcSensor : IDisposable
    {
        
        public UcSensor(int triggerPin, int echoPin)
        {
            Gpio = new GpioController();
            TriggerPin = triggerPin;
            EchoPin = echoPin;

            Gpio.OpenPin(TriggerPin);
            Gpio.OpenPin(EchoPin);

            Gpio.SetPinMode(TriggerPin, PinMode.Output);
            Gpio.SetPinMode(EchoPin, PinMode.Input);

            Gpio.Write(TriggerPin, PinValue.Low);
        }
        

        public double GetDistance()
        {
            var mre = new ManualResetEvent(false);
            mre.WaitOne(500);
            var pulseLength = new Stopwatch();

            // send pulse
            Gpio.Write(TriggerPin, PinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
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
            Debug.WriteLine(timeBetween.ToString());
            var distance = timeBetween.TotalSeconds * 17000;

            return distance;
        }
        

        public void Dispose()
        {
            Gpio?.Dispose();
            Log.Information("Disposed the `_gpio` instance.");
        }

        
        private GpioController Gpio { get; }
        private int TriggerPin { get; }
        private int EchoPin { get; }
        
    }
}