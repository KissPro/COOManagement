using COO.Application.Config.Config;
using COO.Application.Config.Plant;
using COO.Application.MainFuction.DeliverySale;
using COO.Application.MainFuction.BoomEcus;
using COO.Data.EF;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Quartz;
using Quartz.Spi;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using COO.Application.Config.CountryShip;
using Microsoft.EntityFrameworkCore.Internal;

namespace COO.Service
{
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
            Log.Information("=== Get list ecus success!, Total record:", listEcusTs.Count());
            var uploadResult = await _ecusService.InsertList(listEcusTs);
            if (uploadResult == 1)
            {
                Log.Information("========== Collect Ecus TS : Successfully ==========");
            }
            await Task.CompletedTask;
        }
    }
}
