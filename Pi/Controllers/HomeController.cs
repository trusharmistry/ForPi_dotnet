using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pi.Models;

namespace Pi.Controllers
{
    public class DistanceMeasured
    {
        public DistanceMeasured(double distance, string exceptionMessage)
        {
            Distance = distance;
            ExceptionMessage = exceptionMessage;
        }

        public string WithUnit => Distance + "cm";

        private double Distance { get; }

        public string ExceptionMessage { get; }
    }


    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            DistanceMeasured distanceMeasured;
            try
            {
                var distMeasuredBySensor = new DistanceSensor(
                        triggerPin: 18,
                        echoPin: 24)
                    .Measure();
                distanceMeasured = new DistanceMeasured(distMeasuredBySensor, null);
            }
            catch (Exception e)
            {
                distanceMeasured = new DistanceMeasured(0, e.Message);
            }

            return View(model: distanceMeasured);
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}