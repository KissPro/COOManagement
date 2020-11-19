using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using COO.Application.MainFuction.BoomEcus;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.Utilities.Helper;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;

namespace COO.BackendApi.Controllers
{
    [Route("api/boom-ecus")]
    [ApiController]
    public class BoomEcusController : ControllerBase
    {
        private readonly IBoomEcusService _boomEcus;
        public BoomEcusController(IBoomEcusService boomEcusService) {
            _boomEcus = boomEcusService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var boomEcus = await _boomEcus.GetListAll();
                return Ok(boomEcus);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpPost("all-list")]
        public async Task<IActionResult> GetAllList([FromBody]DTParameterModel dtParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var searchBy = dtParameters.Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "TenHang";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns.ElementAt(dtParameters.Order.FirstOrDefault().Column).Data;
                orderAscendingDirection = dtParameters.Order.FirstOrDefault().Dir.ToString().ToLower() == "asc";
            }

            IQueryable<TblBoomEcus> list = (await _boomEcus.GetListAll()).AsQueryable();
            var totalResultsCount = list.Count();

            // filter
            if (!string.IsNullOrEmpty(searchBy))
            {
                foreach (var item in searchBy.Split(';').ToList())
                {

                    // check each line
                    var listCheck = list.Where(x =>
                                          x.TenHang.ToLower().Contains(item.ToLower())
                                       || x.SoTk.ToLower().Contains(item.ToLower())
                                       || x.Country.ToLower().Contains(item.ToLower())
                                       || x.Plant.ToLower().Contains(item.ToLower())
                                       || x.MaHS.ToLower().Contains(item.ToLower())
                                       || x.Quantity.ToString().ToLower().Contains(item.ToLower())
                                       || x.DonGiaHd.ToString().ToLower().Contains(item.ToLower())
                                       || x.NgayDk.ToString().ToLower().Contains(item.ToLower())
                                       || x.Level.ToString().ToLower().Contains(item.ToLower()));
                    if (listCheck.FirstOrDefault() == null && item != searchBy.Split(';').ToList().LastOrDefault())
                        continue;
                    else
                        list = listCheck;
                }
            }

            list = orderAscendingDirection ? list.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : list.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = list.Count();

            var jsonData = new
            {
                draw = dtParameters.Draw,
                recordsFiltered = filteredResultsCount,
                recordsTotal = totalResultsCount,
                data = list
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToList(),
                start = dtParameters.Start
            };
            return Ok(jsonData);
        }
    }
}