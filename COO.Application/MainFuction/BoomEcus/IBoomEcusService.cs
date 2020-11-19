using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.BoomEcus
{
    public interface IBoomEcusService
    {
        Task<List<TblBoomEcus>> GetListAll();
    }
}
