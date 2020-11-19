using COO.Data.EF;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace COO.ApiIntergration
{
    public class BoomEcusClient : IBoomEcusClient
    {
        private readonly string apiHost;
        private readonly IConfiguration _configuration;
        public BoomEcusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            apiHost = _configuration["apiHost"];
        }
        public async Task<List<ViewBoomEcus>> GetAll()
        {
            return await (apiHost + "/api/boom-ecus/all").ConfigureRequest(a => a.Timeout = TimeSpan.FromMinutes(10)).GetJsonAsync<List<ViewBoomEcus>>();
        }
    }
}
