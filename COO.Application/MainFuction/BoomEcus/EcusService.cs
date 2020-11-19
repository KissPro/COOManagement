using COO.Data.EF;
using COO.Utilities.Exceptions;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.BoomEcus
{
    public class EcusService : IEcusService
    {
        private readonly COOContext _context;
        public EcusService(COOContext context)
        {
            _context = context;
        }
        public async Task<Guid> Create(TblEcusTs request)
        {
            _context.TblEcusTs.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }

        public async Task<int> Delete(Guid id)
        {
            var ecusTs = await _context.TblEcusTs.FindAsync(id);
            if (ecusTs == null)
                throw new COOException($"Can not find a ecusTs: {id}");

            _context.TblEcusTs.Remove(ecusTs);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<TblEcusTs>> GetListAll()
        {
            var listEcusTs = await _context.TblEcusTs.ToListAsync();
            return listEcusTs;
        }

        public async Task<TblEcusTs> GetById(Guid id)
        {
            var ecusTs = await _context.TblEcusTs.FindAsync(id);
            if (ecusTs == null) throw new COOException($"Can not find a ecusTs: {id}");
            return ecusTs;
        }

        public async Task<int> InsertList(List<TblEcusTs> listEcusTs)
        {
            try
            {
                if (listEcusTs == null || listEcusTs.Count == 0)
                    return 0;
                await _context.BulkInsertAsync(listEcusTs ,new BulkConfig { BulkCopyTimeout = 1000000 });
            }
            catch (Exception ex)
            {
                return -1;
                throw new COOException($"Error insert list ecusTs:" + ex);
            }
            return 1;
        }
    }
}
