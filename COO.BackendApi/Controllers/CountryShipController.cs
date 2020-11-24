using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using COO.Application.Config.CountryShip;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OfficeOpenXml;
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
        
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update([FromRoute] Guid Id, [FromBody] TblCountryShip request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = Id;
            var affectResult = await _countryService.Update(Id, request);
            if (affectResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var affectResult = await _countryService.Delete(Id);
            if (affectResult == 0)
                return BadRequest();
            return Ok();
        }

        /// <summary>
        /// Upload Excel from Angular
        /// </summary>
        /// <param name="result">userId, path</param>
        /// <returns></returns>
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel([FromBody]FileRespondModel result)
        {
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(result.path)))
                {
                    // Excel to list
                    List<TblCountryShip> list = new List<TblCountryShip>();
                    ExcelWorksheet ws = package.Workbook.Worksheets.First();
                    for (int i = 2; i <= ws.Dimension.End.Row; i++)
                    {
                        if (!string.IsNullOrEmpty(ws.Cells[i, 1].Text))
                            list.Add(new TblCountryShip
                            {
                                Id = Guid.NewGuid(),
                                HMDShipToCode = ws.Cells[i, 1].Text,
                                HMDShipToParty = ws.Cells[i, 2].Text,
                                ShipToCountryCode = ws.Cells[i, 3].Text,
                                ShipToCountryName = ws.Cells[i, 4].Text,
                                RemarkCountry = ws.Cells[i, 5].Text,
                                UpdatedBy = result.userId,
                                UpdatedDate = DateTime.Now,
                            });
                    }
                    // Upload list
                    return Ok(await _countryService.InsertList(list));
                }
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }
    }
}
