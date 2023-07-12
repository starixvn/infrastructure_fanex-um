using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ONEbook.UM.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ONEbook.UM.Site.AspNetCore5.Models;

namespace ONEbook.UM.Site.AspNetCore5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [SkipUMFilter]
        public string Index()
        {
            return "Home";
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

        public string UM() => "UM Page";
    }
}
