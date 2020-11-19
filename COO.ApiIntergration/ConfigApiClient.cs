using COO.Data.EF;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace COO.ApiIntergration
{
    public class ConfigApiClient : IConfigApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly string apiHost;

        public ConfigApiClient(IConfiguration configuration)
        {
            _configuration = configuration;

            apiHost = _configuration["apiHost"];
        }
        public async Task<List<TblCountryShip>> GetAll()
        {
             return await (apiHost + "/api/country/all").GetJsonAsync<List<TblCountryShip>>();
        }
    }
}
