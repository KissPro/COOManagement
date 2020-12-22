using COO.Application.Config.Config;
using COO.Application.Config.Plant;
using COO.Application.MainFuction.DeliverySale;
using COO.Application.MainFuction.BoomEcus;
using COO.Data.EF;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using COO.Application.Config.CountryShip;
using Microsoft.EntityFrameworkCore.Internal;

namespace COO.ServiceSAP
{
    public class CollectListDS_Boom : IJob
    {
        // Inject Service
        private readonly IDeliverySaleService _dsService;
        private readonly IConfigService _configService;
        private readonly IPlantService _plantService;
        private readonly ICountryShipService _countryService;
        private readonly IBoomService _boomService;
        private readonly IDeliverySaleService _deliverySale;

        public CollectListDS_Boom(IDeliverySaleService dsService, IConfigService configService, IPlantService plantService, ICountryShipService countryService, IBoomService boomService, IDeliverySaleService deliverySale)
        {
            _dsService = dsService;
            _configService = configService;
            _plantService = plantService;
            _countryService = countryService;
            _boomService = boomService;
            _deliverySale = deliverySale;
        }

        //public IConfigurationRoot GetConfiguration()
        //{
        //    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //    return builder.Build();
        //}

        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("========== Delivery Sale Running Collect Data. ==========");
            // Collect Data
            List<TblDeliverySales_Temp> listDS = new List<TblDeliverySales_Temp>();

            try
            {
                // Get DN datatable
                // 1. Plant get from list plant table
                var listPlant = await _plantService.GetListAll();
                // 2. Time range collect data from config table
                var lastMonth = Convert.ToInt32((await _configService.GetValueByKey("DstimeLastMonth")).ToString());
                var nextMonth = Convert.ToInt32((await _configService.GetValueByKey("DstimeNextMonth")).ToString());
                var lastYear = Convert.ToInt32((await _configService.GetValueByKey("DstimeLastYear")).ToString());
                var nextYear = Convert.ToInt32((await _configService.GetValueByKey("DstimeNextYear")).ToString());
                DateTime firstDate = DateTime.Now.AddMonths(-lastMonth).AddYears(-lastYear);
                DateTime lastDate = DateTime.Now.AddMonths(nextMonth).AddYears(nextYear);
                // 3. Only get HMD ship to code in country ship table
                var listHMDShipToCode = (await _countryService.GetListAll()).Select(x => x.HMDShipToCode.ToString().Trim()).ToList();
                Log.Information("=== Get config success!");

                foreach (var item in listPlant) // collect all Plant
                {
                    FlurlClient flurlClient = new FlurlClient("http://sap-api-dev.fushan.fihnbb.com/api/DS/get");
                    //FlurlClient flurlClient = new FlurlClient("http://localhost:2779/api/DS/get");

                    flurlClient.Configure(settings => settings.Timeout = TimeSpan.FromHours(1));

                    var result = await flurlClient.Request("")
                        .PostJsonAsync(new
                        {
                            plant = item.Plant,
                            from = firstDate,
                            to = lastDate.AddMinutes(5)
                        }).ReceiveString();

                    if (result == "")
                        continue;

                    //write string to file

                    //System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\JsonFileTemp.txt", result);
                    //string result = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\JsonFileTemp.txt");
                    Log.Information($"=== Result success! . Plant: {item.Plant}, From: {firstDate}, To: {lastDate.AddMinutes(5)}");
                    //.GetStringAsync();
                    var obj = JsonConvert.DeserializeObject<DataTable>(result);
                    DataTable listDSTable = obj;
                    Log.Information($"=== Total record: {listDSTable.Rows.Count}");

                    foreach (DataRow dn in listDSTable.Rows)
                    {
                        if (
                                !String.IsNullOrEmpty(dn["HMD_SHIPTO_CODE"].ToString().Trim()) &&
                                (listHMDShipToCode.FirstOrDefault(x => x.Contains(dn["HMD_SHIPTO_CODE"].ToString().Trim())) != null) &&
                                !String.IsNullOrEmpty(dn["DELIVERY"].ToString().Trim()) &&
                                !String.IsNullOrEmpty(dn["INVOICE_NUMBER_1ST"].ToString().Trim()) &&
                                !String.IsNullOrEmpty(dn["MATERIAL"].ToString().Trim()) &&
                                !String.IsNullOrEmpty(dn["AC_GI_DATE"].ToString().Trim()) &&
                                !String.IsNullOrEmpty(dn["PL_GI_DATE"].ToString().Trim()) &&
                                !String.IsNullOrEmpty(dn["PGI_SYSTEM_DATE"].ToString().Trim()) &&
                                !String.IsNullOrEmpty(dn["DN_QTY"].ToString().Trim()) &&
                                !String.IsNullOrEmpty(dn["NET_PRICE"].ToString().Trim())
                            )
                        {
                            //var hoang = Convert.ToInt64(dn["DELIVERY"].ToString());
                            //var InvoiceNo = (long)Convert.ToDouble(dn["INVOICE_NUMBER_1ST"].ToString().Trim());
                            //var MaterialParent = dn["MATERIAL"].ToString();
                            //var MaterialDesc = dn["MATERIAL_DESCRIPTION"].ToString();
                            //var ShipToCountry = dn["SHIP_TO_COUNTRY"].ToString();
                            //var PartyName = dn["HMD_SHIPTO_PARTY_NAME"].ToString();

                            //var hoang123 = dn["BOLNR"].ToString().Trim();
                            //var CustomerInvoiceNo = (!String.IsNullOrEmpty(dn["BOLNR"].ToString().Trim()) && dn["BOLNR"].ToString().Trim() != "`") ? (long)Convert.ToDouble(dn["BOLNR"].ToString().Trim()) : 0;
                            //var SaleUnit = dn["SALES_UNIT"].ToString();
                            //var ActualGidate = Convert.ToDateTime(dn["AC_GI_DATE"].ToString());
                            //var NetValue = Convert.ToDecimal(dn["NET_VALUE_IN_DOC_CURR"].ToString());
                            //var Dnqty = (long)Convert.ToDouble(dn["DN_QTY"].ToString());
                            //var NetPrice = Convert.ToDecimal(dn["NET_PRICE"].ToString());
                            //var PlanGidate = Convert.ToDateTime(dn["PL_GI_DATE"].ToString());
                            //var PlanGisysDate = Convert.ToDateTime(dn["PGI_SYSTEM_DATE"].ToString());
                            var ds = new TblDeliverySales_Temp()
                            {
                                Id = Guid.NewGuid(),
                                Delivery = (long)Convert.ToDouble(dn["DELIVERY"].ToString()),
                                InvoiceNo = (long)Convert.ToDouble(dn["INVOICE_NUMBER_1ST"].ToString()),
                                MaterialParent = dn["MATERIAL"].ToString(),
                                MaterialDesc = dn["MATERIAL_DESCRIPTION"].ToString(),
                                PartyName = dn["HMD_SHIPTO_PARTY_NAME"].ToString(),
                                CustomerInvoiceNo = (!String.IsNullOrEmpty(dn["BOLNR"].ToString().Trim()) && dn["BOLNR"].ToString().Trim() != "`") ? (long)Convert.ToDouble(dn["BOLNR"].ToString().Trim()) : (long?)null,
                                SaleUnit = dn["SALES_UNIT"].ToString(),
                                ActualGidate = Convert.ToDateTime(dn["AC_GI_DATE"].ToString()),
                                NetValue = Convert.ToDecimal(dn["NET_VALUE_IN_DOC_CURR"].ToString()),
                                Dnqty = (long)Convert.ToDouble(dn["DN_QTY"].ToString()),
                                NetPrice = Convert.ToDecimal(dn["NET_PRICE"].ToString()),
                                PlanGidate = Convert.ToDateTime(dn["PL_GI_DATE"].ToString()),
                                PlanGisysDate = Convert.ToDateTime(dn["PGI_SYSTEM_DATE"].ToString()),
                                Plant = item.Plant,
                                HarmonizationCode = dn["HS_CODE"].ToString(),
                                Address = dn["WE_ZIP_CODE"].ToString() + ';' + dn["WE_ZIP_CITY"].ToString() + ';' + dn["WE_STREET"].ToString() + ';' + dn["WE_UF3"].ToString(),
                                HMDShipToCode = dn["HMD_SHIPTO_CODE"].ToString().Trim(),
                                ShipToCountryName = dn["HMD_SHIPTO_COUNTRY_NAME"].ToString().ToUpper(),
                                ShipToCountry = await _countryService.GetCountryByName(dn["HMD_SHIPTO_COUNTRY_NAME"].ToString().ToUpper()), // GET from config table
                                InsertedDate = DateTime.Now,
                                Status = 0, // Incoming
                            };
                            listDS.Add(ds);
                        }
                    }
                }
                var uploadResultDS = await _dsService.InsertListDN(listDS);

                //var uploadResultDS = 1;

                if (uploadResultDS == 1)
                {
                    Log.Information("========== Collect Delivery and Sales : Successfully ==========");

                    Log.Information("========== Boom Running Collect Data. ==========");
                    // Collect Data
                    List<TblBoom> listDSBoom = new List<TblBoom>();

                    try
                    {
                        // Get Boom datatable
                        // 1.Plant and Material get from DS table
                        var parentMaterialList = (await _deliverySale.GetListAll()).Select(x => new { parentMaterial = x.MaterialParent, plant = x.Plant }).Distinct().ToList();
                        Log.Information($"=== Get list parent material success!, Total Record: {parentMaterialList.Count}");
                        foreach (var item in parentMaterialList) // collect all Plant
                        {
                            FlurlClient flurlClient = new FlurlClient("http://sap-api-dev.fushan.fihnbb.com/api/Boom/get");
                            //FlurlClient flurlClient = new FlurlClient("http://localhost:2779/api/DS/get");

                            flurlClient.Configure(settings => settings.Timeout = TimeSpan.FromHours(1));

                            if (String.IsNullOrEmpty(item.plant.ToString().Trim()) || String.IsNullOrEmpty(item.parentMaterial.ToString().Trim()))
                                continue;

                            var result = await flurlClient.Request("")
                                .PostJsonAsync(new
                                {
                                    plant = item.plant.ToString().Trim(),
                                    parentMaterial = item.parentMaterial.ToString().Trim(),
                                }).ReceiveString();

                            if (result == "")
                                continue;

                            var obj = JsonConvert.DeserializeObject<DataTable>(result);

                            DataTable listDSTable = obj;

                            foreach (DataRow dn in listDSTable.Rows)
                            {
                                if (
                                        !String.IsNullOrEmpty(dn["MATNR"].ToString().Trim()) &&
                                        !String.IsNullOrEmpty(dn["IDNRK"].ToString().Trim()) &&
                                        !String.IsNullOrEmpty(dn["MENGE"].ToString().Trim()) &&
                                        !String.IsNullOrEmpty(dn["WERKS"].ToString().Trim()) &&
                                        !String.IsNullOrEmpty(dn["DESC"].ToString().Trim())
                                    )
                                {
                                    //var ParentMaterial = dn["MATNR"].ToString();
                                    //var Material = dn["IDNRK"].ToString();
                                    //var Quantity = (int)Convert.ToDecimal(dn["MENGE"].ToString());
                                    //var SortString = dn["SORTF"].ToString();
                                    //var AltGroup = dn["ALPGR"].ToString();
                                    //var Plant = dn["WERKS"].ToString();
                                    //var Description = dn["DESC"].ToString();
                                    var ds = new TblBoom()
                                    {
                                        Id = Guid.NewGuid(),
                                        ParentMaterial = dn["MATNR"].ToString(),
                                        Material = dn["IDNRK"].ToString(),
                                        Quantity = (dn["MENGE"].ToString() != "") ? (int)Convert.ToDecimal(dn["MENGE"].ToString()) : 0,
                                        SortString = dn["SORTF"].ToString(),
                                        AltGroup = dn["ALPGR"].ToString(),
                                        Plant = dn["WERKS"].ToString(),
                                        Item = dn["POSNR"].ToString(),
                                        Level = dn["STUFE"].ToString(),
                                        Description = dn["DESC"].ToString(),
                                        InsertedDate = DateTime.Now
                                    };
                                    listDSBoom.Add(ds);
                                }
                            }
                        }
                        Log.Information($"=== Get Boom ok, Total Record: {listDSBoom.Count}");
                        await _boomService.DeleteAll();
                        var uploadResult = await _boomService.InsertList(listDSBoom);

                        // Insert view to new table
                        var viewResult = await _boomService.InsertView();

                        if (uploadResult == 1 && viewResult == true)
                        {
                            Log.Information("========== Collect Boom : Successfully ==========");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Have problems when collect data: " + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Have problems when collect data: " + ex.ToString());
            }
            await Task.CompletedTask;
        }
    }
}
