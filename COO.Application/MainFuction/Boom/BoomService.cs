using COO.Data.EF;
using COO.Utilities.Exceptions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.Boom
{
    public class BoomService : IBoomService
    {
        private readonly COOContext _context;
        public BoomService(COOContext context)
        {
            _context = context;
        }
        public async Task<Guid> Create(TblBoom request)
        {
            _context.TblBoom.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }

        public async Task<int> Delete(Guid id)
        {
            var boom = await _context.TblBoom.FindAsync(id);
            if (boom == null)
                throw new COOException($"Can not find a boom: {id}");

            _context.TblBoom.Remove(boom);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<TblBoom>> GetListAll()
        {
            var listboom = await _context.TblBoom.ToListAsync();
            return listboom;
        }

        public async Task<TblBoom> GetById(Guid id)
        {
            var boom = await _context.TblBoom.FindAsync(id);
            if (boom == null) throw new COOException($"Can not find a boom: {id}");
            return boom;
        }

        public async Task<int> InsertList(List<TblBoom> listboom)
        {
            try
            {
                if (listboom == null || listboom.Count == 0)
                    return 0;
                await _context.BulkInsertAsync(listboom);
            }
            catch (Exception ex)
            {
                return -1;
                throw new COOException($"Error insert list boom:" + ex);
            }
            return 1;
        }
    }
}
