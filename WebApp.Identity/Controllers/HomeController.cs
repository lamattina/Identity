using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Identity.Helpers;
using WebApp.Identity.Models;

namespace WebApp.Identity.Controllers
{
    public class HomeController : Controller
    {
        //private SmtpSettings SmtpSettings { get; set; }

        public HomeController(/*IOptions<SmtpSettings> settings*/)
        {
            //SmtpSettings = settings.Value;
        }

        [Authorize(Policy = "EmployeeId")]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize]
        public IActionResult Privacy()
        {
            //MessageHelper.SendMail(SmtpSettings);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
