using COO.Data.EF;
using COO.Utilities.Exceptions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.DeliverySale
{
    public class DeliverySaleService : IDeliverySaleService
    {
        private readonly COOContext _context;
        public DeliverySaleService(COOContext context)
        {
            _context = context;
        }
        public async Task<Guid> Create(TblDeliverySales request)
        {
            _context.TblDeliverySales.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }

        public async Task<int> Delete(Guid id)
        {
            var deliverySale = await _context.TblDeliverySales.FindAsync(id);
            if (deliverySale == null)
                throw new COOException($"Can not find a deliverySale: {id}");

            _context.TblDeliverySales.Remove(deliverySale);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<TblDeliverySales>> GetListAll()
        {
            var listdeliverySale = await _context.TblDeliverySales.ToListAsync();
            return listdeliverySale;
        }

        public async Task<TblDeliverySales> GetById(Guid id)
        {
            var deliverySale = await _context.TblDeliverySales.FindAsync(id);
            if (deliverySale == null) throw new COOException($"Can not find a deliverySale: {id}");
            return deliverySale;
        }

        public async Task<int> InsertList(List<TblDeliverySales> listdeliverySale)
        {
            try
            {
                if (listdeliverySale == null || listdeliverySale.Count == 0)
                    return 0;

                await _context.BulkInsertAsync(listdeliverySale, new BulkConfig { BulkCopyTimeout = 1000000 });
            }
            catch (Exception ex)
            {
                throw new COOException($"Error insert list deliverySale:" + ex);
            }
            return 1;
        }

        public async Task<int> UpdateStatus(Guid id, int status)
        {
            try
            {
                var deliverySale = await _context.TblDeliverySales.FindAsync(id);
                deliverySale.Status = status;
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new COOException($"Error insert list deliverySale:" + ex);
            }
        }
    }
}
