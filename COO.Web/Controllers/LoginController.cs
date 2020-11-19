using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using COO.ApiIntergration;
using COO.Utilities.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace COO.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;

        public LoginController(IUserApiClient userApiClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _userApiClient = userApiClient;
        }

        public IActionResult Index()
        {
            string login_uri = _configuration["ADWeb_URI"]
                               + "/adweb/oauth2/authorization/v1?scope=read&redirect_uri="
                               + HttpUtility.UrlEncode(_configuration["CLIENT_REDIRECT_URL"])
                               + "&response_type=code&client_id="
                               + _configuration["CLIENT_ID"]
                               + "&state=online";
            return Redirect(login_uri);
        }

        public async Task<IActionResult> Success(string code, string state)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var TokenResult = await _userApiClient.GetAccessToken(code);
                if (TokenResult != null)
                {
                    var userResult = await _userApiClient.GetUserInfor(TokenResult);
                    if (userResult != null)
                    {
                        HttpContext.Session.SetString(SystemConstants.AppSettings.Token, JsonConvert.SerializeObject(userResult));

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", TokenResult);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            string logout = @"http://idmgt.fushan.fihnbb.com/web/session/logout?redirect=" + _configuration["CLIENT_REDIRECT_URL"];
            return Redirect(logout);
        }
    }
}