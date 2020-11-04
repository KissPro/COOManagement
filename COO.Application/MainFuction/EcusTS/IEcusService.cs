using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.EcusTS
{
    public interface IEcusService
    {
        Task<List<TblEcusTs>> GetListAll();
        Task<Guid> Create(TblEcusTs request);
        Task<int> Delete(Guid id);
        Task<TblEcusTs> GetById(Guid id);
        Task<int> InsertList(List<TblEcusTs> listEcusTs);
    }
}
