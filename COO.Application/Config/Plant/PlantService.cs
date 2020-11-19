using COO.Data.EF;
using COO.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.Config.Plant
{
    public class PlantService : IPlantService
    {
        private readonly COOContext _context;

        public PlantService(COOContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(TblPlant request)
        {
            _context.TblPlant.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }

        public async Task<int> Delete(Guid id)
        {
            var plant = await _context.TblPlant.FindAsync(id);
            if (plant == null)
                throw new COOException($"Can not find a plant: {id}");

            _context.TblPlant.Remove(plant);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<TblPlant>> GetListAll()
        {
            var listplant = await _context.TblPlant.ToListAsync();
            return listplant;
        }

        public async Task<TblPlant> GetById(Guid id)
        {
            var plant = await _context.TblPlant.FindAsync(id);
            if (plant == null) throw new COOException($"Can not find a plant: {id}");

            return plant;
        }

        public async Task<int> Update(Guid id, TblPlant request)
        {
            var plant = await _context.TblPlant.FindAsync(id);
            if (plant == null) throw new COOException($"Can not find a plant: {request.Id}");
            plant.Plant = request.Plant;
            plant.UpdatedBy = request.UpdatedBy;
            plant.UpdatedDate = request.UpdatedDate;
            plant.RemarkPlant = request.RemarkPlant;
            return await _context.SaveChangesAsync();
        }
    }
}
