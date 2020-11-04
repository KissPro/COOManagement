using COO.Data.EF;
using COO.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public async Task<int> Update(TblCountryShip request)
        {
            var country = await _context.TblCountryShip.FindAsync(request.Id);
            if (country == null) throw new COOException($"Can not find a country: {request.Id}");

            country.ShipId = request.ShipId;
            country.CountryName = request.CountryName;
            country.RemarkCountry = request.RemarkCountry;
            country.UpdatedBy = request.UpdatedBy;
            country.UpdatedDate = request.UpdatedDate;
            return await _context.SaveChangesAsync();
        }
    }
}
