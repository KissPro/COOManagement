using System;
using System.Collections.Generic;
using System.Text;

namespace COO.ViewModels.Common
{
    public class Header
    {
        public string model { get; set; }
        public int action_id { get; set; }
        public int menu_id { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
    }
    public class CommonModel
    {
        public Header header { get; set; }
        public List<List<object>> data { get; set; }
    }
    public class EmployeeModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string ad_user_employeeID { get; set; }
        public string ad_user_displayName { get; set; }
        public string work_email { get; set; }
        public string job_title { get; set; }
        public string ad_user_sAMAccountName { get; set; }
        public List<object> department_id { get; set; }
        public List<object> parent_id { get; set; }
        public string access_token { get; set; }

        public string department
        {
            get
            {
                try
                {
                    return department_id[1].ToString().Split('/')[0].Trim();
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}
