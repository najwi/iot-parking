using iot_parking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mqtt.Client.AspNetCore.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace iot_parking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMqttClientService _mqtt;

        public HomeController(ILogger<HomeController> logger, ExtarnalService extarnalService)
        {
            _logger = logger;
            _mqtt = extarnalService.GetService();
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult SendMessage()
        {
            _mqtt.SendMessage("clicked");
            return RedirectToAction(nameof(Index));
        }
    }
}
