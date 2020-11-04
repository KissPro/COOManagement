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
        Task<int> Create(BoomCreateRequest request);

        Task<int> Update(BoomCreateRequest requets);
        Task<List<TblBoom>> GetAll();
    }
}
