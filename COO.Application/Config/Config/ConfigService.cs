using COO.Data.EF;
using COO.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COO.Application.Config.Config
{
    public class ConfigService : IConfigService
    {
        private readonly COOContext _context;

        public ConfigService(COOContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create(TblConfig request)
        {
            _context.TblConfig.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }

        public async Task<int> Delete(Guid id)
        {
            var config = await _context.TblConfig.FindAsync(id);
            if (config == null)
                throw new COOException($"Can not find a config: {id}");

            _context.TblConfig.Remove(config);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<TblConfig>> GetListAll()
        {
            var listconfig = await _context.TblConfig.ToListAsync();
            return listconfig;
        }

        public async Task<TblConfig> GetById(Guid id)
        {
            var config = await _context.TblConfig.FindAsync(id);
            if (config == null) throw new COOException($"Can not find a config: {id}");

            return config;
        }

        public async Task<int> Update(Guid id, TblConfig request)
        {
            var config = await _context.TblConfig.FindAsync(id);
            if (config == null) throw new COOException($"Can not find a config: {request.Id}");

            config.EcusRuntime = request.EcusRuntime;
            config.Dsruntime = request.Dsruntime;
            config.DstimeLastMonth = request.DstimeLastMonth;
            config.DstimeNextMonth = request.DstimeNextMonth;
            config.DstimeNextYear = request.DstimeNextYear;
            config.DstimeLastYear = request.DstimeLastYear;
            config.UpdatedBy = config.UpdatedBy;
            config.UpdatedDate = config.UpdatedDate;
            return await _context.SaveChangesAsync();
        }
    }
}
