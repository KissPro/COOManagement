using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace COO.Web.Controllers
{
    public class BaseController : Controller
    {
        protected UserModel mEmployee = null;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessions = context.HttpContext.Session.GetString("User");
            if (sessions == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }
            else
            {
                mEmployee = JsonConvert.DeserializeObject<UserModel>(sessions);
                // Show user name in layout
                ViewBag.User = mEmployee.employee.display_name;
            }
            base.OnActionExecuting(context);
        }
    }
}