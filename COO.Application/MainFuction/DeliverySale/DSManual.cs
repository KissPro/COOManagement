using COO.Data.EF;
using COO.Utilities.Exceptions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.MainFuction.DeliverySale
{
    public class DSManual : IDSManual
    {
        private readonly COOContext _context;
        public DSManual(COOContext context)
        {
            _context = context;
        }

        // Insert Or Update
        public async Task<int> InsertOrUpdateList(List<TblDsmanual> listDS)
        {
            try
            {
                if (listDS == null || listDS.Count == 0)
                    return 0;
                foreach (var item in listDS)
                {
                    var checkCOO = _context.TblDsmanual.FirstOrDefault(x => x.Coono == item.Coono && x.DeliverySales.Delivery == item.DeliverySales.Delivery);
                    if (checkCOO == null)
                    {
                        item.DeliverySalesId = item.DeliverySales.Id;
                        item.DeliverySales = null;
                        _context.TblDsmanual.Add(item);
                    }
                    else
                    {
                        checkCOO.ReceiptDate = item.ReceiptDate;
                        checkCOO.ReturnDate = item.ReturnDate;
                        checkCOO.Cooform = item.Cooform;
                        checkCOO.TrackingDate = item.TrackingDate;
                        checkCOO.TrackingNo = item.TrackingNo;
                        checkCOO.RemarkDs = item.RemarkDs;
                        checkCOO.CourierDate = item.CourierDate;
                        checkCOO.UpdatedBy = item.UpdatedBy;
                        checkCOO.UpdatedDate = item.UpdatedDate;
                        checkCOO.ShipFrom = (item.ShipFrom != "") ? item.ShipFrom : checkCOO.ShipFrom;
                        checkCOO.Package = (item.Package != "") ? item.Package : checkCOO.Package;
                    }
                    await _context.SaveChangesAsync();
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw new COOException($"Error insert list deliverySale:" + ex);
            }
        }

        public async Task<bool> RemoveManual(TblDsmanual dsManual)
        {
            try
            {
                var check = await _context.TblDsmanual.FirstOrDefaultAsync(x => x.Coono == dsManual.Coono && x.DeliverySalesId == dsManual.DeliverySales.Id);
                _context.TblDsmanual.Remove(check);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new COOException($"Error remove deliverySale:" + ex);
            }
        }

        public async Task<List<TblDsmanual>> GetListCreated()
        {
            var listCreated = await _context.TblDsmanual.Where(x =>
                            x.ReceiptDate == null ||
                            x.ReturnDate == null ||
                            x.Cooform == null ||
                            x.TrackingNo == null ||
                            x.CourierDate == null ||
                            x.TrackingDate == null ||
                            x.UpdatedBy == null ||
                            x.UpdatedDate == null
                ).Include(x => x.DeliverySales).ToListAsync();
            return listCreated;
        }


        public async Task<List<TblDsmanual>> GetListCompleted()
        {
            var listCreated = await _context.TblDsmanual.Where(x =>
                            x.ReceiptDate != null &&
                            x.ReturnDate != null &&
                            x.Cooform != null &&
                            x.TrackingNo != null &&
                            x.CourierDate != null &&
                            x.TrackingDate != null &&
                            x.UpdatedBy != null &&
                            x.UpdatedDate != null
                ).Include(x => x.DeliverySales).ToListAsync();
            return listCreated;
        }

        public async Task<int> InsertIncoming(TblDsmanual dsManual)
        {
            try
            {
                _context.Add(dsManual);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        public async Task<List<TblDsmanual>> GetListByCOO(string cooNo)
        {
            try
            {
                return await _context.TblDsmanual.Where(x => x.Coono.Trim() == cooNo.Trim()).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
                throw new COOException("GetListByCOO Error: ", ex);
            }
        }
    }
}
