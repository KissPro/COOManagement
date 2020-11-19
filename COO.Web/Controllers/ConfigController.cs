using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.ApiIntergration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace COO.Web.Controllers
{
    [Route("config")]
    public class ConfigController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigApiClient _config;

        public ConfigController(IConfiguration configuration, IConfigApiClient config)
        {
            _config = config;
            _configuration = configuration;
        }
        [HttpGet("country-show")]
        public async Task<IActionResult> ShowCountryShip()
        {
            ViewBag.ListCountryShip = await _config.GetAll();
            return View();
        }
    }
}