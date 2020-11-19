using COO.Data.EF;
using COO.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.BoomEcus
{
    public class BoomEcusService : IBoomEcusService
    {
        private readonly COOContext _context;
        public BoomEcusService(COOContext context)
        {
            _context = context;
            _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
        }
        public async Task<List<TblBoomEcus>> GetListAll()
        {
            try
            {
                var listboomEcus = await _context.TblBoomEcus.ToListAsync();
                return listboomEcus;
            }
            catch (Exception ex)
            {
                throw new COOException("GetListAll", ex);
            }
        }
    }
}
