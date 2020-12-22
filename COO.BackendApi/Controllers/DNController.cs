using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using COO.Application.MainFuction.DeliverySale;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.Utilities.Helper;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace COO.BackendApi.Controllers
{
    [Route("api/dn")]
    [ApiController]
    public class DNController : ControllerBase
    {
        private readonly IDeliverySaleService _ds;
        private readonly IDSManual _manual;
        public DNController(IDeliverySaleService ds, IDSManual dSManual)
        {
            _ds = ds;
            _manual = dSManual;
        }

        //[HasCredential(Permissions = "CanRead")]
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

        [HttpGet("all-manual")]
        public async Task<IActionResult> GetCreatedList()
        {
            try
            {
                var listManual = await _manual.GetListCreated();
                return Ok(listManual);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpPost("list-manual/{type}")]
        public async Task<IActionResult> GetAllListManual([FromBody]DTParameterModel dtParameters, [FromRoute]string type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var searchBy = dtParameters.Search?.Value;

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "UpdatedDate";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns.ElementAt(dtParameters.Order.FirstOrDefault().Column).Data;
                orderAscendingDirection = dtParameters.Order.FirstOrDefault().Dir.ToString().ToLower() == "asc";
            }

            IQueryable<TblDsmanual> list = (type != "completed") ? (await _manual.GetListCreated()).AsQueryable() : (await _manual.GetListCompleted()).AsQueryable();
            var totalResultsCount = list.Count();

            // filter
            if (!string.IsNullOrEmpty(searchBy))
            {
                foreach (var item in searchBy.Split(';').ToList())
                {

                    // check each line
                    var listCheck = list.Where(x =>
                                          x.Coono.ToString().ToLower().Contains(item.ToLower())
                                       || x.ReceiptDate.GetValueOrDefault().ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || x.ReturnDate.GetValueOrDefault().ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || ((!String.IsNullOrEmpty(x.Cooform)) && x.Cooform.ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.TrackingNo)) && x.TrackingNo.ToLower().Contains(item.ToLower()))
                                       || x.CourierDate.GetValueOrDefault().ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || x.TrackingDate.GetValueOrDefault().ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || ((!String.IsNullOrEmpty(x.Origin)) && x.Origin.ToString().ToLower().Contains(item.ToLower()))
                                       || ((!String.IsNullOrEmpty(x.UpdatedBy)) && x.UpdatedBy.ToString().ToLower().Contains(item.ToLower()))
                                       || x.UpdatedDate.GetValueOrDefault().ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || ((!String.IsNullOrEmpty(x.RemarkDs)) && x.RemarkDs.ToString().ToLower().Contains(item.ToLower()))
                                       || x.Package.ToString().ToLower().Contains(item.ToLower())
                                       || x.ShipFrom.ToString().ToLower().Contains(item.ToLower())
                                       // DN information
                                       || x.DeliverySales.PartyName.ToString().ToLower().Contains(item.ToLower())
                                       || x.DeliverySales.InvoiceNo.ToString().ToLower().Contains(item.ToLower())
                                       || x.DeliverySales.CustomerInvoiceNo.ToString().ToLower().Contains(item.ToLower())
                                       || x.DeliverySales.ShipToCountry.ToString().ToLower().Contains(item.ToLower())
                                       || x.DeliverySales.Delivery.ToString().ToLower().Contains(item.ToLower())
                                     );

                    if (listCheck.FirstOrDefault() == null)
                        continue;
                    else
                        list = listCheck;
                }
            }

            list = orderAscendingDirection ? list.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : list.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = list.Count();
            // add list to session for download
            if (type != "completed")
                HttpContext.Session.SetString("ListDNM", JsonConvert.SerializeObject(list));
            else
                HttpContext.Session.SetString("ListDNM_Completed", JsonConvert.SerializeObject(list));


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
                                       || x.ShipToCountryName.ToLower().Contains(item.ToLower())
                                       || x.HMDShipToCode.ToLower().Contains(item.ToLower())
                                       || x.PartyName.ToString().ToLower().Contains(item.ToLower())
                                       || x.CustomerInvoiceNo.ToString().ToLower().Contains(item.ToLower())
                                       || x.SaleUnit.ToString().ToLower().Contains(item.ToLower())
                                       || x.ActualGidate.ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || x.NetValue.ToString().ToLower().Contains(item.ToLower())
                                       || x.Dnqty.ToString().ToLower().Contains(item.ToLower()) // ok
                                       || x.NetPrice.ToString().ToLower().Contains(item.ToLower())
                                       || x.PlanGidate.ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || x.PlanGisysDate.ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower())
                                       || x.InsertedDate.ToString().ToLower().Contains(item.ToLower())
                                       || x.UpdatedDate.GetValueOrDefault().ToString("dd/MM/yyyy").ToLower().Contains(item.ToLower()) // ok
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
            // add list to session for download
            HttpContext.Session.SetString("ListDN", JsonConvert.SerializeObject(list));

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

        [HttpGet("download-dn")]
        public async Task<IActionResult> DownloadListDN()
        {
            try
            {
                var listDN = JsonConvert.DeserializeObject<List<TblDeliverySales>>(HttpContext.Session.GetString("ListDN"));
                if (listDN == null) return BadRequest();
                using (var excelPackage = new ExcelPackage())
                {
                    var workbook = excelPackage.Workbook;
                    var workSheet = workbook.Worksheets.Add("Sheet1");
                    // Header
                    workSheet.Cells[1, 1].Value = "#";
                    workSheet.Cells[1, 2].Value = "Delivery";
                    workSheet.Cells[1, 3].Value = "Invoice No";
                    workSheet.Cells[1, 4].Value = "MaterialParent";
                    workSheet.Cells[1, 5].Value = "MaterialDesc";
                    workSheet.Cells[1, 6].Value = "ShipToCountry";
                    workSheet.Cells[1, 7].Value = "PartyName";
                    workSheet.Cells[1, 8].Value = "CustomerInvoiceNo";
                    workSheet.Cells[1, 9].Value = "SaleUnit";
                    workSheet.Cells[1, 10].Value = "ActualGIDate";
                    workSheet.Cells[1, 11].Value = "NetValue";
                    workSheet.Cells[1, 12].Value = "DNQty";
                    workSheet.Cells[1, 13].Value = "NetPrice";
                    workSheet.Cells[1, 14].Value = "PlanGIDate";
                    workSheet.Cells[1, 15].Value = "PlanGISysDate";
                    workSheet.Cells[1, 16].Value = "InsertedDate";
                    workSheet.Cells[1, 17].Value = "UpdatedDate";
                    workSheet.Cells[1, 18].Value = "Status";
                    workSheet.Cells[1, 19].Value = "Zip Code";
                    workSheet.Cells[1, 20].Value = "City";
                    workSheet.Cells[1, 21].Value = "Street";
                    workSheet.Cells[1, 22].Value = "Adress";
                    workSheet.Cells[1, 23].Value = "HarmonizationCode";
                    workSheet.Cells[1, 24].Value = "Plant";
                    workSheet.Cells[1, 25].Value = "HMDShipToCode";
                    workSheet.Cells[1, 26].Value = "ShipToCountryName";
                    // Data
                    for (int i = 0; i < listDN.Count; i++)
                    {
                        var item = listDN[i];
                        workSheet.Cells[i + 2, 1].Value = i + 1;
                        workSheet.Cells[i + 2, 2].Value = item.Delivery;
                        workSheet.Cells[i + 2, 3].Value = item.InvoiceNo;
                        workSheet.Cells[i + 2, 4].Value = item.MaterialParent;
                        workSheet.Cells[i + 2, 5].Value = item.MaterialDesc;
                        workSheet.Cells[i + 2, 6].Value = item.ShipToCountry;
                        workSheet.Cells[i + 2, 7].Value = item.PartyName;
                        workSheet.Cells[i + 2, 8].Value = item.CustomerInvoiceNo;
                        workSheet.Cells[i + 2, 9].Value = item.SaleUnit;
                        workSheet.Cells[i + 2, 10].Value = (item.ActualGidate == null) ? "" : item.ActualGidate.ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 11].Value = item.NetValue;
                        workSheet.Cells[i + 2, 12].Value = item.Dnqty;
                        workSheet.Cells[i + 2, 13].Value = item.NetPrice;
                        workSheet.Cells[i + 2, 14].Value = item.PlanGidate.ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 15].Value = item.PlanGisysDate.ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 16].Value = item.InsertedDate.ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 17].Value = (item.UpdatedDate == null) ? "" : item.UpdatedDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 18].Value = (item.Status == 0) ? "Incoming" : "Created";
                        workSheet.Cells[i + 2, 19].Value = item.Address.Split(";")[0];
                        workSheet.Cells[i + 2, 20].Value = item.Address.Split(";")[1];
                        workSheet.Cells[i + 2, 21].Value = item.Address.Split(";")[2];
                        workSheet.Cells[i + 2, 22].Value = item.Address.Split(";")[3];
                        workSheet.Cells[i + 2, 23].Value = item.HarmonizationCode;
                        workSheet.Cells[i + 2, 24].Value = item.Plant;
                        workSheet.Cells[i + 2, 25].Value = item.HMDShipToCode;
                        workSheet.Cells[i + 2, 26].Value = item.ShipToCountryName;
                    }
                    // Border
                    workSheet.Cells[1, 1, 1, 26].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1, 1, 26].Style.Font.Bold = true;


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

        [HttpGet("download-manual/{type}")]
        public async Task<IActionResult> DownloadListDNManual([FromRoute]string type)
        {
            try
            {
                var listDNM = (type != "completed") ? JsonConvert.DeserializeObject<List<TblDsmanual>>(HttpContext.Session.GetString("ListDNM"))
                                              : JsonConvert.DeserializeObject<List<TblDsmanual>>(HttpContext.Session.GetString("ListDNM_Completed"));
                if (listDNM == null) return BadRequest();
                using (var excelPackage = new ExcelPackage())
                {
                    var workbook = excelPackage.Workbook;
                    var workSheet = workbook.Worksheets.Add("Sheet1");
                    // Header
                    workSheet.Cells[1, 1].Value = "#";
                    workSheet.Cells[1, 2].Value = "Name";
                    workSheet.Cells[1, 3].Value = "Code";
                    workSheet.Cells[1, 4].Value = "DNN";
                    workSheet.Cells[1, 5].Value = "Invoice No";
                    workSheet.Cells[1, 6].Value = "Customer Invoice";
                    workSheet.Cells[1, 7].Value = "Doc Recept Date";
                    workSheet.Cells[1, 8].Value = "COO No";
                    workSheet.Cells[1, 9].Value = "Return Date";
                    workSheet.Cells[1, 10].Value = "COO Form";
                    workSheet.Cells[1, 11].Value = "Tracking No";
                    workSheet.Cells[1, 12].Value = "Tracking Date";
                    workSheet.Cells[1, 13].Value = "Courier Date";
                    workSheet.Cells[1, 14].Value = "Ship From";
                    workSheet.Cells[1, 15].Value = "Package";
                    workSheet.Cells[1, 16].Value = "Remark";
                    // Data
                    for (int i = 0; i < listDNM.Count; i++)
                    {
                        var item = listDNM[i];
                        workSheet.Cells[i + 2, 1].Value = i + 1;
                        workSheet.Cells[i + 2, 2].Value = item.DeliverySales.PartyName;
                        workSheet.Cells[i + 2, 3].Value = item.DeliverySales.ShipToCountry;
                        workSheet.Cells[i + 2, 4].Value = item.DeliverySales.Delivery;
                        workSheet.Cells[i + 2, 5].Value = item.DeliverySales.InvoiceNo;
                        workSheet.Cells[i + 2, 6].Value = item.DeliverySales.CustomerInvoiceNo;
                        workSheet.Cells[i + 2, 7].Value = (item.ReceiptDate == null) ? "" : item.ReceiptDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 8].Value = item.Coono;
                        workSheet.Cells[i + 2, 9].Value = (item.ReturnDate == null) ? "" : item.ReturnDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 10].Value = item.Cooform;
                        workSheet.Cells[i + 2, 11].Value = item.TrackingNo;
                        workSheet.Cells[i + 2, 12].Value = (item.TrackingDate == null) ? "" : item.TrackingDate.GetValueOrDefault().ToString("yyyy-MM-dd"); 
                        workSheet.Cells[i + 2, 13].Value = (item.CourierDate == null) ? "" : item.CourierDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        workSheet.Cells[i + 2, 14].Value = item.ShipFrom;
                        workSheet.Cells[i + 2, 15].Value = item.Package;
                        workSheet.Cells[i + 2, 16].Value = item.RemarkDs;
                    }
                    // Border
                    workSheet.Cells[1, 1, 1, 16].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1, 1, 16].Style.Font.Bold = true;


                    var memory = await Task.Run(() => new MemoryStream(excelPackage.GetAsByteArray()));
                    memory.Position = 0;
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"); // excel type .xlsx
                }
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpGet("open-coo/{cooNo}")]
        public async Task<IActionResult> GetListDNByCOO([FromRoute]string cooNo)
        {
            try
            {
                List<TblDeliverySales> listDN = new List<TblDeliverySales>();
                var listId = await _manual.GetListByCOO(cooNo);
                foreach (var item in listId)
                {
                    var ds = await _ds.GetById(item.DeliverySalesId);
                    listDN.Add(ds);
                }
                return Ok(listDN);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpPost("export-coo")]
        public async Task<IActionResult> ExportCOO([FromBody]ExportCOOModel result)
        {
            try
            {
                string templatePath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "TemplateFile"), "COO_Template.xlsx");
                using (ExcelPackage package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    // 0. Common
                    worksheet.Cells[1, 24].Value = result.CooNo.ToString().Trim();
                    worksheet.Cells[17, 6].Value = result.Ship.ToString().Trim();  // From 
                    worksheet.Cells[18, 6].Value = result.Dn.FirstOrDefault().ShipToCountryName;  // To
                    worksheet.Cells[22, 2].Value = result.PackageNo.ToString().Trim() + " PACKAGE";
                    worksheet.Cells[47, 28].Value = result.Dn.FirstOrDefault().ShipToCountryName;


                    // 1. Product Information
                    List<string> hsCodeList = new List<string>();
                    for (int i = 0; i < result.Dn.Count; i++)
                    {
                        if (!hsCodeList.Contains(result.Dn[i].HarmonizationCode))
                        {
                            hsCodeList.Add(result.Dn[i].HarmonizationCode);
                        }
                        worksheet.Cells[24 + i, 2].Value = "CELL PHONE " + result.Dn[i].MaterialParent + " " + result.Dn[i].MaterialDesc;
                        worksheet.Cells[24 + i, 25].Value = result.Dn[i].Dnqty; // PSC
                    }
                    // Hs code - mutiple - unique
                    worksheet.Cells[22, 13].Value = String.Join(" ", hsCodeList.ToArray());

                    // 2. Invoice Information
                    List<TblDeliverySales> listDNUnique = result.Dn
                                                                .GroupBy(m => new { m.Delivery, m.InvoiceNo })
                                                                .Select(group => group.First())  // instead of First you can also apply your logic here what you want to take, for example an OrderBy
                                                                .ToList();
                    for (int i = 0; i < listDNUnique.Count; i++)
                    {
                        worksheet.Cells[24 + i * 2, 30].Value = listDNUnique[i].CustomerInvoiceNo;
                        worksheet.Cells[24 + i * 2 + 1, 30].Value = listDNUnique[i].ActualGidate.AddHours(-8).ToString("dd.MMM.yyyy");
                        worksheet.Cells[24 + listDNUnique.Count * 2 + i * 2 + 1, 30].Value = listDNUnique[i].InvoiceNo;
                        worksheet.Cells[24 + listDNUnique.Count * 2 + i * 2 + 2, 30].Value = listDNUnique[i].PlanGisysDate.AddHours(-8).ToString("dd.MMM.yyyy");
                    }
                    worksheet.Cells[24 + listDNUnique.Count * 2, 30].Value = "Invoice Viet Nam Ref:";

                    // 3. Adress Ship To Informatiom
                    string address = result.Dn.FirstOrDefault().Address;
                    worksheet.Cells[8, 2].Value = $"{result.Dn.FirstOrDefault().PartyName}" +
                                                $"\n{address.Split(';')[3]}" +
                                                $"\n{address.Split(';')[2]}" +
                                                $"\n{address.Split(';')[0]} {address.Split(';')[1]}" +
                                                $"\n{result.Dn.FirstOrDefault().ShipToCountryName}";


                    var memory = await Task.Run(() => new MemoryStream(package.GetAsByteArray()));
                    memory.Position = 0;
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"); // excel type .xlsx
                }
            }
            catch (Exception ex)
            {
                throw new COOException("Error:", ex);
            }
        }

        [HttpPost("update-manual")]
        public async Task<IActionResult> UpdateManual([FromBody]TblDsmanual ds)
        {
            try
            {
                List<TblDsmanual> listManual = new List<TblDsmanual>();
                listManual.Add(ds);
                await _manual.InsertOrUpdateList(listManual);
                return Ok();
            }
            catch (Exception ex)
            {
                throw new COOException("Error:", ex);
            }
        }

        [HttpPost("remove-manual")]
        public async Task<IActionResult> RemoveManual([FromBody]TblDsmanual ds)
        {
            try
            {
                // Remove in list manual
                await _manual.RemoveManual(ds);
                // Change status 1 -> 0: incoming
                await _ds.UpdateStatus(ds.DeliverySalesId, 0);
                return Ok();
            }
            catch (Exception ex)
            {
                throw new COOException("Error:", ex);
            }
        }

        [HttpPost("save-coo")]
        public async Task<IActionResult> SaveCOO([FromBody]ExportCOOModel result)
        {
            try
            {
                List<TblDsmanual> listManual = new List<TblDsmanual>();
                foreach (var item in result.Dn)
                {
                    // 1. Change state of COO: 0: Incoming -> 1: COO Created
                    await _ds.UpdateStatus(item.Id, 1);
                    // 2. Save all manualy input data
                    var ds = new TblDsmanual()
                    {
                        Id = Guid.NewGuid(),
                        DeliverySalesId = item.Id,
                        DeliverySales = item,
                        Coono = result.CooNo,
                        Package = result.PackageNo,
                        ShipFrom = result.Ship,
                    };
                    listManual.Add(ds);
                    //await _manual.InsertIncoming(ds);
                }
                //await _manual.InsertList(listManual);

                await _manual.InsertOrUpdateList(listManual);

                return Ok();
            }
            catch (Exception ex)
            {
                throw new COOException("Error:", ex);
            }
        }

        /// <summary>
        /// Upload Excel from Angular
        /// </summary>
        /// <param name="result">userId, path</param>
        /// <returns></returns>
        [HttpPost("import-excel")]
        [Authorize]
        public async Task<IActionResult> ImportExcel([FromBody]FileRespondModel result)
        {
            try
            {
                //var pathFile = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("UploadedFile", "Plant")), result.path);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(result.path)))
                {
                    // Excel to list
                    List<TblDsmanual> list = new List<TblDsmanual>();
                    ExcelWorksheet ws = package.Workbook.Worksheets.First();
                    for (int i = 2; i <= ws.Dimension.End.Row; i++)
                    {
                        if (!string.IsNullOrEmpty(ws.Cells[i, 1].Text)
                            && !string.IsNullOrEmpty(ws.Cells[i, 2].Text)
                            && !string.IsNullOrEmpty(ws.Cells[i, 3].Text)
                            && !string.IsNullOrEmpty(ws.Cells[i, 4].Text)
                            && !string.IsNullOrEmpty(ws.Cells[i, 5].Text)
                            && !string.IsNullOrEmpty(ws.Cells[i, 6].Text)
                            && !string.IsNullOrEmpty(ws.Cells[i, 7].Text)
                            && !string.IsNullOrEmpty(ws.Cells[i, 8].Text))
                        {
                            TblDeliverySales checkDs = new TblDeliverySales();
                            // Check update or insert
                            var parentMaterial = ws.Cells[i, 12].Text.ToString();
                            var ds = await _ds.GetByDN(Convert.ToInt64(ws.Cells[i, 1].Text), ws.Cells[i, 12].Text.ToString());
                            // If insert and ds is not null
                            if (!String.IsNullOrEmpty(parentMaterial) && ds != null)
                                checkDs = ds;
                            // If update
                            else if (String.IsNullOrEmpty(parentMaterial))
                                checkDs.Delivery = Convert.ToInt64(ws.Cells[i, 1].Text);
                            else
                                break;
                            list.Add(new TblDsmanual
                            {
                                Id = Guid.NewGuid(),
                                DeliverySales = checkDs,
                                Coono = ws.Cells[i, 2].Text.Trim(),
                                ReceiptDate = Convert.ToDateTime(ws.Cells[i, 3].Text),
                                ReturnDate = Convert.ToDateTime(ws.Cells[i, 4].Text),
                                Cooform = ws.Cells[i, 5].Text.Trim(),
                                TrackingNo = ws.Cells[i, 6].Text.Trim(),
                                CourierDate = Convert.ToDateTime(ws.Cells[i, 7].Text),
                                TrackingDate = Convert.ToDateTime(ws.Cells[i, 8].Text),
                                RemarkDs = ws.Cells[i, 9].Text,
                                ShipFrom = ws.Cells[i, 10].Text,
                                Package = ws.Cells[i, 11].Text,
                                UpdatedBy = result.userId,
                                UpdatedDate = DateTime.Now,
                            }); ;
                        }
                    }
                    // Upload list
                    return Ok(await _manual.InsertOrUpdateList(list));
                }
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }
    }
}
