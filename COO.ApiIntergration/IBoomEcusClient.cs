using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.ApiIntergration
{
    public interface IBoomEcusClient
    {
        Task<List<ViewBoomEcus>> GetAll();
    }
}
