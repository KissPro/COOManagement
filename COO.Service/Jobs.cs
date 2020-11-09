using COO.Application.Common;
using COO.Application.Config.Config;
using COO.Application.Config.Plant;
using COO.Application.MainFuction.DeliverySale;
using COO.Application.MainFuction.EcusTS;
using COO.Data.EF;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using Quartz.Spi;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace COO.Service
{
    public class CollectListBoom : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("================= It's Work.");
            await Task.CompletedTask;
        }
    }


    public class CollectListEcusTS : IJob
    {
        // Inject Service
        private readonly IEcusService _ecusService;

        public CollectListEcusTS(IEcusService ecusService)
        {
            _ecusService = ecusService;
        }

        //public IConfigurationRoot GetConfiguration()
        //{
        //    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //    return builder.Build();
        //}

        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("========== Ecus TS Running Collect Data. ==========");
            // Collect Data
            //string connectionString = GetConfiguration().GetSection("ConnectionStrings").GetSection("EcusTS").Value;
            string connectionString = @"Server=hvecusp01;Database=TEST;User Id=sa;Password=F1hnbb@vn;MultipleActiveResultSets=true";

            List<TblEcusTs> listEcusTs = new List<TblEcusTs>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = @" select a.SOTK,								
                                a.MA_HANGKB  ,								
                                a.MA_NPL_SP  ,								
                                a.TEN_HANG   ,								
                                a.DGIA_HDTM  ,								
                                a.TEN_NUOC_XX,b.NGAY_DK from [DHANGMDDK] a left join [DTOKHAIMD] b on a.SOTK = b.SOTK								
                                Where a.SOTK is not null								
                                and b.NGAY_DK is not null		
                            ";
                SqlCommand myCommand = new SqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader = myCommand.ExecuteReader())
                    {
                        // Loop over the results
                        //DataTable dt = new DataTable();
                        //dt.Load(dataReader);

                        while (dataReader.Read())
                        {
                            decimal? donGia = null;
                            if (!String.IsNullOrEmpty(dataReader["DGIA_HDTM"].ToString()))
                            {
                                float donGiaFloat = float.Parse(dataReader["DGIA_HDTM"].ToString());
                                donGia = Convert.ToDecimal(donGiaFloat);
                            }
                            var ecusNew = new TblEcusTs()
                            {
                                Id = Guid.NewGuid(),
                                SoTk = Convert.ToString(dataReader["SOTK"].ToString()),
                                NgayDk = Convert.ToDateTime(dataReader["NGAY_DK"].ToString()),
                                MaHang = Convert.ToString(dataReader["MA_NPL_SP"].ToString()),
                                MaHs = Convert.ToString(dataReader["MA_HANGKB"].ToString()),
                                TenHang = Convert.ToString(dataReader["TEN_HANG"].ToString()),
                                DonGiaHd = donGia,
                                Country = Convert.ToString(dataReader["TEN_NUOC_XX"].ToString()),
                                InsertedDate = DateTime.Now
                            };
                            listEcusTs.Add(ecusNew);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Have problems when collect data: " + ex.ToString());
                }
                finally
                {
                    connection.Close();
                }
            }
            var uploadResult = await _ecusService.InsertList(listEcusTs);
            if (uploadResult == 1)
            {
                Log.Information("========== Collect Ecus TS : Successfully ==========");
            }
            await Task.CompletedTask;
        }
    }


    public class CollectListDS : IJob
    {
        // Inject Service
        private readonly IDeliverySaleService _dsService;
        private readonly ISAPService _sapService;
        private readonly IConfigService _configService;
        private readonly IPlantService _plantService;

        public CollectListDS(IDeliverySaleService dsService, IConfigService configService, IPlantService plantService, ISAPService sapService)
        {
            _dsService = dsService;
            _sapService = sapService;
            _configService = configService;
            _plantService = plantService;
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
            List<TblDeliverySales> listDS = new List<TblDeliverySales>();

            try
            {
                // Get DN datatable/
                // 1. Plant get from list plant table
                // 2. Time range collect data from config table
                var listPlant = await _plantService.GetListAll();
                var lastMonth = Convert.ToInt32((await _configService.GetListAll()).FirstOrDefault().DstimeLastMonth.ToString());
                var nextMonth = Convert.ToInt32((await _configService.GetListAll()).FirstOrDefault().DstimeNextMonth.ToString());
                var lastYear = Convert.ToInt32((await _configService.GetListAll()).FirstOrDefault().DstimeLastYear.ToString());
                var nextYear = Convert.ToInt32((await _configService.GetListAll()).FirstOrDefault().DstimeNextYear.ToString());
                DateTime firstDate = DateTime.Now.AddMonths(-lastMonth).AddYears(-lastYear);
                DateTime lastDate = DateTime.Now.AddMonths(nextMonth).AddYears(nextYear);

                foreach (var item in listPlant) // collect all Plant
                {
                    var check = _sapService.TestSAPCore();
                    DataTable listDSTable = _sapService.DownloadDeliverySale(item.Plant.Trim(), firstDate, lastDate);
                    foreach (DataRow dn in listDSTable.Rows)
                    {
                        var ds = new TblDeliverySales()
                        {
                            Id = Guid.NewGuid(),
                            Delivery = Convert.ToInt64(dn["DELIVERY"].ToString()),
                            InvoiceNo = Convert.ToInt64(dn["INVOICE_NUMBER_1ST"].ToString()),
                            MaterialParent = dn["MATERIAL"].ToString(),
                            MaterialDesc = dn["MATERIAL_DESCRIPTION"].ToString(),
                            ShipToCountry = dn["SHIP_TO_COUNTRY"].ToString(),
                            PartyName = dn["HMD_SHIPTO_PARTY_NAME"].ToString(),
                            CustomerInvoiceNo = Convert.ToInt64(dn["BillOfLad./ERS No.??"].ToString()),
                            SaleUnit = dn["SALES_UNIT"].ToString(),
                            ActualGidate = Convert.ToDateTime(dn["AC_GI_DATE"].ToString()),
                            NetValue = Convert.ToDecimal(dn["NET_VALUE_IN_DOC_CURR"].ToString()),
                            Dnqty = Convert.ToInt64(dn["DN_QTY"].ToString()),
                            NetPrice = Convert.ToDecimal(dn["NET_PRICE"].ToString()),
                            PlanGidate = Convert.ToDateTime(dn["PL_GI_DATE"].ToString()),
                            PlanGisysDate = Convert.ToDateTime(dn["PGI_SYSTEM_DATE"].ToString()),
                            //Address COO
                            //HarmonizationCode
                            InsertedDate = DateTime.Now
                        };
                        listDS.Add(ds);
                    }
                }
                var uploadResult = await _dsService.InsertList(listDS);
                if (uploadResult == 1)
                {
                    Log.Information("========== Collect Delivery and Sales : Successfully ==========");
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
