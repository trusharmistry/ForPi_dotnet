using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pi
{
    
    internal class TimedHostedService : IHostedService, IDisposable
    {
        
       /// <summary>
       /// Constructor initialize the logger and configuration object with minimum distance.
       /// </summary>
        public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration configuration)
        {
            if (!(configuration is null)
                && double.TryParse(configuration.GetSection("Distance:Minimum").Value, out var minDistInCm))
                MinDistInCm = minDistInCm;
            else
                MinDistInCm = 30.0;

            _logger = logger;
        }


       /// <summary>
       /// Start timer.
       /// </summary>
       /// <param name="cancellationToken"></param>
       /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        
        /// <summary>
        /// Cancel timer.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        
        /// <inheritdoc cref="Dispose"/>
        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }

        
        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            var id = DataAccess.GetLastRecordId();
            var distMeasuredBySensor = DistanceSensor.Measure(18, 24);
            var res = distMeasuredBySensor < MinDistInCm
                ? id == Guid.Empty ? DataAccess.Insert() : 0 // insert only if there is no open record
                : id == Guid.Empty
                    ? 0
                    : DataAccess.Update(id); // update only if there is open record

            _logger.LogInformation($"Timed Background Service is added {res} record.");
        }
        
        
        private double MinDistInCm { get; }

        private readonly ILogger _logger;
        
        private Timer _timer;

    }
    
}