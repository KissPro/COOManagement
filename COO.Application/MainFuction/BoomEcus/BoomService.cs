using COO.Data.EF;
using COO.Utilities.Exceptions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.BoomEcus
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

        [Obsolete]
        public async Task<bool> DeleteAll()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[tbl_Boom]");
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new COOException("Error: ", ex);
            }
        }

        [Obsolete]
        public async Task<bool> InsertView()
        {
            try
            {
                string cmd = @"
                                truncate table [dbo].[tbl_BoomEcusTS]
                                insert into [dbo].[tbl_BoomEcusTS](
                                    [MaHS],[Quantity],[DonGiaHD],[Country],[SoTK],[NgayDK],[ParentMaterial],[SortString]
                                    ,[AltGroup],[Plant],[TenHang],[Level],[Item])
                                select 
                                    [MaHS],[Quantity],[DonGiaHD],[Country],[SoTK],[NgayDK],[ParentMaterial],[SortString]
                                    ,[AltGroup],[Plant],[TenHang],[Level],[Item] 
                                from [dbo].[v_BoomEcusTS]
                             ";
                 _context.Database.ExecuteSqlCommand(cmd);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new COOException("Error: ", ex);
            }
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
                await _context.BulkInsertAsync(listboom, new BulkConfig { BulkCopyTimeout = 1000000 });
            }
            catch (Exception ex)
            {
                throw new COOException($"Error insert list boom:" + ex);
            }
            return 1;
        }
    }
}
