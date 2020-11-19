using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using COO.Application.Config.CountryShip;
using COO.Data.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SQLitePCL;

namespace COO.BackendApi.Controllers
{
    [Route("api/country")]
    [ApiController]
    public class CountryShipController : ControllerBase
    {
        private readonly ICountryShipService _countryService;
        public CountryShipController(ICountryShipService countryService) { _countryService = countryService; }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var country = await _countryService.GetListAll();
            return Ok(country);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var country = await _countryService.GetById(Id);
            if (country == null)
                return BadRequest("Can not find country");
            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TblCountryShip request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countryId = await _countryService.Create(request);
            if (countryId == null)
                return BadRequest();
            var country = await _countryService.GetById(countryId);

            return CreatedAtAction(nameof(GetById), new { id = countryId }, country);
        }
        
        [HttpPut("{countryId}")]
        public async Task<IActionResult> Update([FromRoute] Guid countryId, [FromBody] TblCountryShip request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = countryId;
            var affectResult = await _countryService.Update(countryId, request);
            if (affectResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{countryId}")]
        public async Task<IActionResult> Delete(Guid countryId)
        {
            var affectResult = await _countryService.Delete(countryId);
            if (affectResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}