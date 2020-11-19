using COO.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.ApiIntergration
{
    public interface IUserApiClient
    {
        Task<string> GetAccessToken(string code);
        Task<UserModel> GetUserInfor(string accessToken);

        Task<EmployeeModel> GetDetailUserInfor(string accessToken, string empID);
    }
}
