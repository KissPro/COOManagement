using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.ApiIntergration;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.Utilities.Helper;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace COO.Web.Controllers
{
    [Route("boom-ecus")]
    public class BoomEcusController : BaseController
    {
        private readonly IBoomEcusClient _boomEcusClient;
        public BoomEcusController(IBoomEcusClient boomEcusClient)
        {
            _boomEcusClient = boomEcusClient;
        }

        [HttpGet("all")]
        public IActionResult ShowBoomEcus()
        {
            //ViewBag.ListBoomEcus = await _boomEcusClient.GetAll();
            return View();
        }

        [HttpPost("add")]
        public int Add(int number1)
        {
            return number1;
        }

        [HttpPost("show-all")]
        public async Task<IActionResult> AjaxShowBoomEcus()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                // Default value
                string sortColumnName = "TenHang";
                string sortDirection = "asc";

                // Server Side Parameter
                int start = Convert.ToInt32(Request.Form["start"]);
                int length = Convert.ToInt32(Request.Form["length"]);
                string searchValue = Request.Form["search[value]"];
                sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
                sortDirection = Request.Form["order[0][dir]"];

                IQueryable<ViewBoomEcus> list = (await _boomEcusClient.GetAll()).AsQueryable();

                int totalrows = await list.CountAsync();
                // filter
                if (!string.IsNullOrEmpty(searchValue))
                {
                    foreach (var item in searchValue.Split(';').ToList())
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
                        if (listCheck.FirstOrDefault() == null && item != searchValue.Split(';').ToList().LastOrDefault())
                            continue;
                        else
                            list = listCheck;
                    }
                }
                IQueryable<ViewBoomEcus> listAfter = list.OrderBy(x => x.TenHang);
                int totalrowsafterfiltering = await listAfter.CountAsync();

                //if (!(string.IsNullOrEmpty(sortColumnName)) && !string.IsNullOrEmpty(sortDirection))
                //{
                //    switch (sortColumnName)
                //    {
                //        case "SoTK": listAfter = (sortDirection == "asc") ? listAfter.OrderBy(emp => emp.SoTk) : listAfter.OrderByDescending(emp => emp.SoTk); break;
                //        case "MaHS": listAfter = (sortDirection == "asc") ? listAfter.OrderBy(emp => emp.MaHS) : listAfter.OrderByDescending(emp => emp.MaHS); break;
                //        case "Quantity": listAfter = (sortDirection == "asc") ? listAfter.OrderBy(emp => emp.Quantity) : listAfter.OrderByDescending(emp => emp.Quantity); break;
                //        case "DonGiaHD": listAfter = (sortDirection == "asc") ? listAfter.OrderBy(emp => emp.DonGiaHd) : listAfter.OrderByDescending(emp => emp.DonGiaHd); break;
                //        case "Country": listAfter = (sortDirection == "asc") ? listAfter.OrderBy(emp => emp.Country) : listAfter.OrderByDescending(emp => emp.Country); break;
                //        case "NgayDK": listAfter = (sortDirection == "asc") ? listAfter.OrderBy(emp => emp.NgayDk) : listAfter.OrderByDescending(emp => emp.NgayDk); break;
                //        default: listAfter = sortDirection == "asc" ? listAfter.OrderBy(emp => emp.TenHang) : listAfter.OrderByDescending(emp => emp.TenHang); break;
                //    }
                //}
                listAfter = sortDirection == "asc" ? listAfter.OrderByDynamic(sortColumnName, DtOrderDir.Asc) : listAfter.OrderByDynamic(sortColumnName, DtOrderDir.Desc);

                TempData["listEvent"] = listAfter;
                // paging
                listAfter = listAfter.Skip(start).Take(length);
                return Ok(new { data = listAfter, draw = Request.Form["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering });

            }
            catch (Exception ex)
            {
                return BadRequest();
                throw new COOException("Error", ex);
            }
        }
    }
}