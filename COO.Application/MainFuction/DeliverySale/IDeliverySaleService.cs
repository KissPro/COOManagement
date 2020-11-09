using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.DeliverySale
{
    public interface IDeliverySaleService
    {
        Task<List<TblDeliverySales>> GetListAll();
        Task<Guid> Create(TblDeliverySales request);
        Task<int> Delete(Guid id);
        Task<TblDeliverySales> GetById(Guid id);
        Task<int> InsertList(List<TblDeliverySales> listDS);

        Task<int> UpdateStatus(Guid id, int status);
    }
}
