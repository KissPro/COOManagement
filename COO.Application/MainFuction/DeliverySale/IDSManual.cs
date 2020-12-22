using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.DeliverySale
{
    public interface IDSManual
    {
        Task<int> InsertOrUpdateList(List<TblDsmanual> listDS);
        Task<int> InsertIncoming(TblDsmanual dsManual);
        Task<bool> RemoveManual(TblDsmanual dsManual);
        Task<List<TblDsmanual>> GetListCreated();
        Task<List<TblDsmanual>> GetListByCOO(string cooNo);
        Task<List<TblDsmanual>> GetListCompleted();
    }
}
