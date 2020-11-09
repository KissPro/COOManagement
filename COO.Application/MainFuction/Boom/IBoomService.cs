using COO.Data.EF;
using COO.ViewModels.MainFunction.Boom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace COO.Application.MainFuction.Boom
{
    public interface IBoomService
    {
        Task<List<TblBoom>> GetListAll();
        Task<Guid> Create(TblBoom request);
        Task<int> Delete(Guid id);
        Task<TblBoom> GetById(Guid id);
        Task<int> InsertList(List<TblBoom> listBoom);
    }
}
