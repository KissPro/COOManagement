using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.Config.Plant
{
    public interface IPlantService
    {
        Task<List<TblPlant>> GetListAll();
        Task<Guid> Create(TblPlant request);
        Task<int> Update(Guid id, TblPlant request);
        Task<int> Delete(Guid id);
        Task<TblPlant> GetById(Guid id);

    }
}
