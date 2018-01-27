using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace ParentsRules.Controllers
{
    public class StatusCodeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public StatusCodeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/StatusCode/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
           
            _logger.LogInformation($"Unexpected Status Code: {statusCode}");
            return View(statusCode);
        }
    }
}