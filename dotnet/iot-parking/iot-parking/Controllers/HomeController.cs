using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

using iot_parking.Models;
using iot_parking.Services;

namespace iot_parking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMqttClientService _mqtt;

        public HomeController(ILogger<HomeController> logger, IMqttClientService mqttClientService)
        {
            _logger = logger;
            _mqtt = mqttClientService;
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
