using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.Application.Config.Config;
using COO.Application.Config.CountryShip;
using COO.Application.Config.Plant;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace COO.BackendApi.Controllers
{
    [Route("api/config")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _config;
        public ConfigController(IConfigService configService)
        {
            _config = configService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetConfigList()
        {
            try
            {
                var listConfig = await _config.GetListAll();
                return Ok(listConfig);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateConfig([FromBody]TblConfig config)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _config.Create(config);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConfig(Guid id, [FromBody]TblConfig config)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _config.Update(id, config);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfig(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var resul = await _config.Delete(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

    }
}