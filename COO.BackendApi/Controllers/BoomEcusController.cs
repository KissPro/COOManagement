using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json;
using OfficeOpenXml;

namespace COO.BackendApi.Controllers
{
    [Route("api/boom-ecus")]
    [ApiController]
    public class BoomEcusController : ControllerBase
    {
        private readonly IBoomEcusService _boomEcus;
        public BoomEcusController(IBoomEcusService boomEcusService)
        {
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
                                          ((!String.IsNullOrEmpty(x.Item)) && x.Item.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.ParentMaterial)) && x.ParentMaterial.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.AltGroup)) && x.AltGroup.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.SortString)) && x.SortString.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.TenHang)) && x.TenHang.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.SoTk)) && x.SoTk.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.Country)) && x.Country.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.Plant)) && x.Plant.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.MaHS)) && x.MaHS.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.Quantity.ToString())) && x.Quantity.ToString().ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.DonGiaHd.ToString())) && x.DonGiaHd.ToString().ToLower().Contains(item.ToLower()))
                                       || x.NgayDk.GetValueOrDefault().ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || ((!String.IsNullOrEmpty(x.Level)) && x.Level.ToString().ToLower().Contains(item.ToLower()))
                                    );
                    if (listCheck.FirstOrDefault() == null && item != searchBy.Split(';').ToList().LastOrDefault())
                        continue;
                    else
                        list = listCheck;
                }
            }

            list = orderAscendingDirection ? list.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : list.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = list.Count();
            // add list to session for download
            HttpContext.Session.SetString("ListBoomEcus", JsonConvert.SerializeObject(list));

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

        [HttpGet("download")]
        public async Task<IActionResult> Download()
        {
            try
            {
                var listDN = JsonConvert.DeserializeObject<List<TblBoomEcus>>(HttpContext.Session.GetString("ListBoomEcus"));
                if (listDN == null) return BadRequest();
                using (var excelPackage = new ExcelPackage())
                {
                    var workbook = excelPackage.Workbook;
                    var workSheet = workbook.Worksheets.Add("Sheet1");
                    // Header
                    workSheet.Cells[1, 1].Value = "#";
                    workSheet.Cells[1, 2].Value = "Plant";
                    workSheet.Cells[1, 3].Value = "Material Parent";
                    workSheet.Cells[1, 4].Value = "Level";
                    workSheet.Cells[1, 5].Value = "Item";
                    workSheet.Cells[1, 6].Value = "Ten Hang";
                    workSheet.Cells[1, 7].Value = "Ma HS";
                    workSheet.Cells[1, 8].Value = "Quantity";
                    workSheet.Cells[1, 9].Value = "Don Gia";
                    workSheet.Cells[1, 10].Value = "Country";
                    workSheet.Cells[1, 11].Value = "So TK";
                    workSheet.Cells[1, 12].Value = "Ngay DK";
                    workSheet.Cells[1, 13].Value = "Alt Group";
                    workSheet.Cells[1, 14].Value = "Sort String";
                    // Data
                    for (int i = 0; i < listDN.Count; i++)
                    {
                        var item = listDN[i];
                        workSheet.Cells[i + 2, 1].Value = i + 1;
                        workSheet.Cells[i + 2, 2].Value = item.Plant;
                        workSheet.Cells[i + 2, 3].Value = item.ParentMaterial;
                        workSheet.Cells[i + 2, 4].Value = item.Level;
                        workSheet.Cells[i + 2, 5].Value = item.Item;
                        workSheet.Cells[i + 2, 6].Value = item.TenHang;
                        workSheet.Cells[i + 2, 7].Value = item.MaHS;
                        workSheet.Cells[i + 2, 8].Value = item.Quantity;
                        workSheet.Cells[i + 2, 9].Value = item.DonGiaHd;
                        workSheet.Cells[i + 2, 10].Value = item.Country;
                        workSheet.Cells[i + 2, 11].Value = item.SoTk;
                        workSheet.Cells[i + 2, 12].Value = item.NgayDk;
                        workSheet.Cells[i + 2, 13].Value = item.AltGroup;
                        workSheet.Cells[i + 2, 14].Value = item.SortString;
                    }
                    // Border
                    workSheet.Cells[1, 1, 1, 14].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1, 1, 14].Style.Font.Bold = true;


                    var memory = await Task.Run(() => new MemoryStream(excelPackage.GetAsByteArray()));
                    memory.Position = 0;
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"); // excel type .xlsx
                }
            }
            catch (Exception ex)
            {
                throw new COOException("Error:", ex);
            }
        }
    }
}