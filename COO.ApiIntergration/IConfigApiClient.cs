using COO.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.ApiIntergration
{
    public interface IConfigApiClient
    {
        // Country ship
        Task<List<TblCountryShip>> GetAll();
    }
}
