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
        [HttpPost]
        public async Task<IActionResult> CreateConfig([FromBody]TblConfig config)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _config.Create(config);
                if (result == null)
                    return BadRequest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateConfig([FromRoute] Guid Id, [FromBody]TblConfig config)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _config.Update(Id, config);
                if (result == 0)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteConfig(Guid Id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _config.Delete(Id);
                if (result == 0)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

    }
}