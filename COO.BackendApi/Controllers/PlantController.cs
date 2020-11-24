using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using COO.Application.Config.Plant;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using OfficeOpenXml;

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
        [HttpPost]
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
        public async Task<IActionResult> UpdatePlant([FromRoute]Guid id, [FromBody]TblPlant plant)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _plant.Update(id, plant);
                if (result == 0)
                    return BadRequest();
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
                var result = await _plant.Delete(id);
                if (result == 0)
                    return BadRequest();
                return Ok(id);
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        /// <summary>
        /// Upload Excel from Angular
        /// </summary>
        /// <param name="result">userId, path</param>
        /// <returns></returns>
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel([FromBody]FileRespondModel result)
        {
            try
            {
                //var pathFile = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("UploadedFile", "Plant")), result.path);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(result.path)))
                {
                    // Excel to list
                    List<TblPlant> list = new List<TblPlant>();
                    ExcelWorksheet ws = package.Workbook.Worksheets.First();
                    for (int i = 2; i <= ws.Dimension.End.Row; i++)
                    {
                        if (!string.IsNullOrEmpty(ws.Cells[i, 1].Text))
                            list.Add(new TblPlant
                            {
                                Id = Guid.NewGuid(),
                                Plant = ws.Cells[i, 1].Text,
                                UpdatedBy = result.userId,
                                UpdatedDate = DateTime.Now
                            });
                    }
                    // Upload list
                    return Ok(await _plant.InsertList(list));
                }
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }
    }

}