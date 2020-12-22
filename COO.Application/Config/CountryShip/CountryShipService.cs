using COO.Data.EF;
using COO.Utilities.Exceptions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.Config.CountryShip
{
    public class CountryShipService : ICountryShipService
    {
        private readonly COOContext _context;

        public CountryShipService(COOContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(TblCountryShip request)
        {
            _context.TblCountryShip.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }

        public async Task<int> Delete(Guid id)
        {
            var country = await _context.TblCountryShip.FindAsync(id);
            if (country == null)
                throw new COOException($"Can not find a country: {id}");

            _context.TblCountryShip.Remove(country);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<TblCountryShip>> GetListAll()
        {
            var listCountry = await _context.TblCountryShip.ToListAsync();
            return listCountry;
        }

        public async Task<TblCountryShip> GetById(Guid id)
        {
            var country = await _context.TblCountryShip.FindAsync(id);
            if (country == null) throw new COOException($"Can not find a country: {id}");

            return country;
        }

        public async Task<int> Update(Guid id, TblCountryShip request)
        {
            var country = await _context.TblCountryShip.FindAsync(id);
            if (country == null) throw new COOException($"Can not find a country: {request.Id}");

            country.HMDShipToCode = request.HMDShipToCode;
            country.HMDShipToParty = request.HMDShipToParty;
            country.ShipToCountryCode = request.ShipToCountryCode;
            country.ShipToCountryName = request.ShipToCountryName;
            country.RemarkCountry = request.RemarkCountry;
            country.UpdatedBy = request.UpdatedBy;
            country.UpdatedDate = request.UpdatedDate;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> InsertList(List<TblCountryShip> listCountry)
        {
            try
            {
                if (listCountry == null || listCountry.Count == 0)
                    return 0;
                await _context.BulkInsertAsync(listCountry, new BulkConfig { BulkCopyTimeout = 1000000 });
            }
            catch (Exception ex)
            {
                throw new COOException($"Error insert list country:" + ex);
            }
            return 1;
        }

        public async Task<string> GetCountryByName(string countryName)
        {
            try
            {
                var country = await _context.TblCountryShip.FirstOrDefaultAsync();
                return country.ShipToCountryCode;
            }
            catch (Exception ex)
            {
                throw new COOException("Error", ex);
            }
        }
    }
}
