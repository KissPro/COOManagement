using System;
using System.Collections.Generic;
using System.Text;

namespace COO.Utilities.Helper
{
    public class ADWebHelper
    {
        private string ADWeb_URI = @"http://idmgt.fushan.fihnbb.com";
        private string CLIENT_ID = @"vEwSNKfc6USHiiyLA3sR7DuKFFrUlbJVm";
        private string CLIENT_SECRET = @"F1uk98jcuxlUoQPAFsqfrJYF3AZwBm7a";
        private string CLIENT_REDIRECT_URL = @"http://localhost:50349/login/success";
        public static string URI_ADWEB_SEARCH = "/adweb/record/search/v1";

        public ADWebHelper()
        {
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ADWeb_URI"]))
            //    ADWeb_URI = ConfigurationManager.AppSettings["ADWeb_URI"];

            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CLIENT_ID"]))
            //    CLIENT_ID = ConfigurationManager.AppSettings["CLIENT_ID"];

            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CLIENT_SECRET"]))
            //    CLIENT_SECRET = ConfigurationManager.AppSettings["CLIENT_SECRET"];

            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CLIENT_REDIRECT_URL"]))
            //    CLIENT_REDIRECT_URL = ConfigurationManager.AppSettings["CLIENT_REDIRECT_URL"];
        }
    }
}
