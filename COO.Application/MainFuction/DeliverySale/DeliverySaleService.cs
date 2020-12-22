using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.ViewModels.MainFunction;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
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
            // Status = 0 : incoming
            var listdeliverySale = await _context.TblDeliverySales.Where(x => x.Status == 0).ToListAsync();
            return listdeliverySale;
        }

        public async Task<TblDeliverySales> GetById(Guid id)
        {
            var deliverySale = await _context.TblDeliverySales.FindAsync(id);
            if (deliverySale == null) return null;
            return deliverySale;
        }

        public async Task<TblDeliverySales> GetByDN(long dn, string parentMaterial)
        {
            var deliverySale = await _context.TblDeliverySales.FirstOrDefaultAsync(x => x.Delivery == dn && x.MaterialParent == parentMaterial);
            if (deliverySale == null) return null;
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

        [Obsolete]
        public async Task<int> InsertListDN(List<TblDeliverySales_Temp> listdeliverySale)
        {
            try
            {
                // Insert temp table first
                 _context.BulkInsert(listdeliverySale, new BulkConfig { BulkCopyTimeout = 1000000 });
                // Updating destination table, and dropping temp table
                var updateTable = @"update dbo.tbl_DeliverySales set [Status] = null where [Status] = 0
                                                merge into dbo.tbl_DeliverySales as Target
                                                using [dbo].[tbl_DeliverySales_Temp] as Source
                                                on Target.[Delivery] = Source.[Delivery] and Target.[MaterialParent] = Source.[MaterialParent]
                                                when matched and Target.[Status] = 0 then
                                                    update set 
                                                            Target.[InvoiceNo] = Source.[InvoiceNo],
                                                            Target.[MaterialDesc] = Source.[MaterialDesc],
                                                            Target.[ShipToCountry] = Source.[ShipToCountry],
                                                            Target.[PartyName] = Source.[PartyName],
                                                            Target.[CustomerInvoiceNo] = Source.[CustomerInvoiceNo],
                                                            Target.[SaleUnit] = Source.[SaleUnit],
                                                            Target.[ActualGIDate] = Source.[ActualGIDate],
                                                            Target.[NetValue] = Source.[NetValue],
                                                            Target.[DNQty] = Source.[DNQty],
                                                            Target.[NetPrice] = Source.[NetPrice],
                                                            Target.[PlanGIDate] = Source.[PlanGIDate],
                                                            Target.[PlanGISysDate] = Source.[PlanGISysDate],
                                                            Target.[InsertedDate] = Source.[InsertedDate],
                                                            Target.[UpdatedDate] = Source.[UpdatedDate],
                                                            Target.[Status] = Source.[Status],
                                                            Target.[Address] = Source.[Address],
                                                            Target.[HarmonizationCode] = Source.[HarmonizationCode],
                                                            Target.[Plant] = Source.[Plant],
                                                            Target.[HMDShipToCode] = Source.[HMDShipToCode],
                                                            Target.[ShipToCountryName] = Source.[ShipToCountryName]
                                                when not matched then
                                                    insert ([ID],[Delivery],[InvoiceNo],[MaterialParent],[MaterialDesc],[ShipToCountry],[PartyName],
                                                            [CustomerInvoiceNo],[SaleUnit],[ActualGIDate],[NetValue],[DNQty],[NetPrice],[PlanGIDate],
                                                            [PlanGISysDate],[InsertedDate],[UpdatedDate],[Status],[Address],[HarmonizationCode],
                                                            [Plant],[HMDShipToCode],[ShipToCountryName]) 
                                                    values (Source.[ID],Source.[Delivery],Source.[InvoiceNo],Source.[MaterialParent],
                                                            Source.[MaterialDesc],Source.[ShipToCountry],Source.[PartyName],Source.[CustomerInvoiceNo],
                                                            Source.[SaleUnit],Source.[ActualGIDate],Source.[NetValue],Source.[DNQty],Source.[NetPrice],
                                                            Source.[PlanGIDate],Source.[PlanGISysDate],Source.[InsertedDate],Source.[UpdatedDate],
                                                            Source.[Status],Source.[Address],Source.[HarmonizationCode],Source.[Plant],Source.[HMDShipToCode],
                                                            Source.[ShipToCountryName]);
                                               delete dbo.tbl_DeliverySales where [Status] is null
                                               truncate table [dbo].[tbl_DeliverySales_Temp]";
                _context.Database.ExecuteSqlCommand(updateTable);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                throw new COOException($"Error insert list deliverySale:" + ex);
            }
        }

        [Obsolete]
        public async Task<bool> DeleteAll()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[tbl_DeliverySales]");
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new COOException("Error: ", ex);
            }
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
