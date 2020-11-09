using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.Config.Config
{
    public interface IConfigService
    {
        Task<List<TblConfig>> GetListAll();
        Task<Guid> Create(TblConfig request);
        Task<int> Update(TblConfig request);
        Task<int> Delete(Guid id);
        Task<TblConfig> GetById(Guid id);

    }
}
