using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.AspNetCore.Filters;
using System.Diagnostics;
using ONEbook.UM.Site.AspNetCore5.Models;

namespace ONEbook.UM.Site.AspNetCore5.Controllers
{
    public class HomeController : Controller
    {
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
