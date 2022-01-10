using iot_parking.Database;
using iot_parking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

using iot_parking.Services;

namespace iot_parking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMqttClientService _mqtt;
        private readonly DatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, DatabaseContext context, IMqttClientService mqttClientService)
        {
            _logger = logger;
            _context = context;
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
    }
}
