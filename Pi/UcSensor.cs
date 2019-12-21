using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Serilog;

namespace Pi
{
    public class UcSensor:IDisposable
    {

        public UcSensor(int triggerPin, int echoPin)
        {
            _gpio = new GpioController();
            _triggerPin = triggerPin;
            _echoPin = echoPin;
            
            _gpio.OpenPin(_triggerPin);
            Log.Information($"GPIO triggerPin enabled for use: {_triggerPin}");
            _gpio.OpenPin(_echoPin);
            Log.Information($"GPIO echoPin enabled for use: {_echoPin}");

            _gpio.SetPinMode(_triggerPin, PinMode.Output);
            _gpio.SetPinMode(_echoPin, PinMode.Output);

            _gpio.Write(_triggerPin, PinValue.Low);
        }

        public double GetDistance()
        {
            var mre = new ManualResetEvent(false);
            mre.WaitOne(500);
            var pulseLength = new Stopwatch();

            // send pulse
            _gpio.Write(_triggerPin, PinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
            _gpio.Write(_triggerPin, PinValue.Low);

            // receive pulse
            while (_gpio.Read(_echoPin) == PinValue.Low)
            {
            }
            pulseLength.Start();


            while (_gpio.Read(_echoPin) == PinValue.High)
            {
            }
            pulseLength.Stop();

            //Calculating distance
            var timeBetween = pulseLength.Elapsed;
            Debug.WriteLine(timeBetween.ToString());
            var distance = timeBetween.TotalSeconds * 17000;

            return distance;
        }

        public void Dispose()
        {
            Log.Information($"Disposing GPIO instance.");
            _gpio?.Dispose();
            Log.Information($"Disposed GPIO instance.");
        }
        
        private readonly GpioController _gpio;
        private readonly int _triggerPin;
        private readonly int _echoPin;
    }
}