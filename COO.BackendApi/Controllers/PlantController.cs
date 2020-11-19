using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COO.Application.Config.Plant;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace COO.BackendApi.Controllers
{
    [Route("api/plant")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plant;
        public PlantController(IPlantService plant)
        {
            _plant = plant;
        }

        // 2. Plant
        [HttpGet("all")]
        public async Task<IActionResult> GetPlantList()
        {
            try
            {
                var listPlant = await _plant.GetListAll();
                return Ok(listPlant);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreatePlant([FromBody]TblPlant plant)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _plant.Create(plant);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(Guid id, [FromBody]TblPlant plant)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _plant.Update(id, plant);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var resul = await _plant.Delete(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }
    }
}