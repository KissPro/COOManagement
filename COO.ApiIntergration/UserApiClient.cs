using COO.Utilities.Exceptions;
using COO.ViewModels.Common;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace COO.ApiIntergration
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IConfiguration _configuration;
        private string ADWeb_URI = "http://idmgt.fushan.fihnbb.com";
        private string CLIENT_ID = "iTamCf61OHFQQvSgzWBRtQDijutK9czS";
        private string CLIENT_SECRET = "DFHW86lqRTn8zndsilOEJ5cPM3yuNKBG";
        private string CLIENT_REDIRECT_URL = "https://localhost:44399/Login/Success";

        private static string URI_ADWEB_SEARCH = "/adweb/record/search/v1";

        public void InitAdweb()
        {
            if (!string.IsNullOrEmpty(_configuration["ADWeb_URI"]))
                ADWeb_URI = _configuration["ADWeb_URI"];

            if (!string.IsNullOrEmpty(_configuration["CLIENT_ID"]))
                CLIENT_ID = _configuration["CLIENT_ID"];

            if (!string.IsNullOrEmpty(_configuration["CLIENT_SECRET"]))
                CLIENT_SECRET = _configuration["CLIENT_SECRET"];

            if (!string.IsNullOrEmpty(_configuration["CLIENT_REDIRECT_URL"]))
                CLIENT_REDIRECT_URL = _configuration["CLIENT_REDIRECT_URL"];
        }

        public UserApiClient(IConfiguration configuration) {
            _configuration = configuration;
            InitAdweb();
        }

        public async Task<string> GetAccessToken(string code)
        {
            try
            {
                var res = (ADWeb_URI + "/adweb/oauth2/access_token/v1")
                .PostUrlEncodedAsync(new
                {
                    client_id = CLIENT_ID,
                    redirect_uri = CLIENT_REDIRECT_URL,
                    client_secret = CLIENT_SECRET,
                    code = code,
                    grant_type = "authorization_code"
                }).ReceiveString().Result;
                dynamic obj = JsonConvert.DeserializeObject<object>(res);
                if (!string.IsNullOrEmpty(obj.access_token.ToString()))
                {
                    return await Task.Run(() => obj.access_token.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new COOException($"GetAccessToken error: ", ex);
            }
        }

        public async Task<EmployeeModel> GetDetailUserInfor(string accessToken, string empID)
        {
            try
            {
                var res = (ADWeb_URI + URI_ADWEB_SEARCH)
                   .WithOAuthBearerToken(accessToken)
                   .SetQueryParams(new
                   {
                       model = "hr.employee",
                       fields = "[\"id\", \"name\",\"ad_user_employeeID\", \"ad_user_displayName\", \"work_email\", \"job_title\", \"ad_user_sAMAccountName\", \"parent_id\", \"department_id\"]",
                       search_datas = "[('ad_user_employeeID', '=', '" + empID + "')]"
                   })
                   .GetStringAsync().Result;

                var data = JsonConvert.DeserializeObject<List<CommonModel>>(res);
                if (data.Count == 2 && data[1].data != null)
                {
                    dynamic _data = data[1].data[0][0];
                    if (_data.job_title == "Head of Factory")
                    {
                        var _obj = new EmployeeModel
                        {
                            id = _data.id,
                            name = _data.name,
                            ad_user_displayName = _data.ad_user_displayName,
                            ad_user_employeeID = _data.ad_user_employeeID,
                            ad_user_sAMAccountName = _data.ad_user_sAMAccountName,
                            job_title = _data.job_title,
                            work_email = _data.work_email,
                            department_id = new List<object> { "0", "Fushan Factory" },
                            parent_id = new List<object> { _data.id, _data.ad_user_displayName },
                        };
                        return _obj;
                    }
                    var emp = await Task.Run(() =>  JsonConvert.DeserializeObject<EmployeeModel>(data[1].data[0][0].ToString()));
                    return emp;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new COOException($"GetDetailUserInfor error: ", ex);
            }
        }

        public async  Task<UserModel> GetUserInfor(string accessToken)
        {
            try
            {
                var res = (ADWeb_URI + "/adweb/people/me/v1")
                    .WithOAuthBearerToken(accessToken)
                    .GetStringAsync().Result;
                var obj = await Task.Run(()=> JsonConvert.DeserializeObject<UserModel>(res));
                return  obj;
            }
            catch (Exception ex)
            {
                throw new COOException($"GetUserInfor error: ", ex);
            }
        }
    }
}
