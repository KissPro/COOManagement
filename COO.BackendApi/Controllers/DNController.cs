using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.Application.MainFuction.DeliverySale;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.Utilities.Helper;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace COO.BackendApi.Controllers
{
    [Route("api/dn")]
    [ApiController]
    public class DNController : ControllerBase
    {
        private readonly IDeliverySaleService _ds;
        public DNController(IDeliverySaleService ds) {
            _ds = ds;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var boomEcus = await _ds.GetListAll();
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
            var orderCriteria = "Delivery";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns.ElementAt(dtParameters.Order.FirstOrDefault().Column).Data;
                orderAscendingDirection = dtParameters.Order.FirstOrDefault().Dir.ToString().ToLower() == "asc";
            }

            IQueryable<TblDeliverySales> list = (await _ds.GetListAll()).AsQueryable();
            var totalResultsCount = list.Count();

            // filter
            if (!string.IsNullOrEmpty(searchBy))
            {
                foreach (var item in searchBy.Split(';').ToList())
                {

                    // check each line
                    var listCheck = list.Where(x =>
                                          x.Delivery.ToString().ToLower().Contains(item.ToLower())
                                       || x.InvoiceNo.ToString().ToLower().Contains(item.ToLower())
                                       || x.MaterialParent.ToLower().Contains(item.ToLower())
                                       || x.MaterialDesc.ToLower().Contains(item.ToLower())
                                       || x.ShipToCountry.ToLower().Contains(item.ToLower())
                                       || x.PartyName.ToString().ToLower().Contains(item.ToLower())
                                       || x.CustomerInvoiceNo.ToString().ToLower().Contains(item.ToLower())
                                       || x.SaleUnit.ToString().ToLower().Contains(item.ToLower())
                                       || x.ActualGidate.ToString().ToLower().Contains(item.ToLower())
                                       || x.NetValue.ToString().ToLower().Contains(item.ToLower())
                                       || x.Dnqty.ToString().ToLower().Contains(item.ToLower()) // ok
                                       || x.NetPrice.ToString().ToLower().Contains(item.ToLower())
                                       || x.PlanGidate.ToString().ToLower().Contains(item.ToLower())
                                       || x.PlanGisysDate.ToString().ToLower().Contains(item.ToLower())
                                       || x.InsertedDate.ToString().ToLower().Contains(item.ToLower())
                                       || x.UpdatedDate.ToString().ToLower().Contains(item.ToLower()) // ok
                                       || ((!String.IsNullOrEmpty(x.Address)) && x.Address.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.HarmonizationCode)) && x.HarmonizationCode.ToLower().Contains(item.ToLower()))
                                       || x.Plant.ToLower().Contains(item.ToLower())
                                       || x.Status.ToString().ToLower().Contains(item.ToLower()));
                    if (listCheck.FirstOrDefault() == null)
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