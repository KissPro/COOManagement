using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using COO.Data.EF;
using COO.Utilities.Exceptions;
using COO.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace COO.BackendApi.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteFile([FromBody]FileRespondModel result)
        {
            try
            {
                string filePath = result.path;
                if (System.IO.File.Exists(filePath))
                {
                    await Task.Run(() => System.IO.File.Delete(filePath));
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }

        [HttpPost("upload/{type}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile([FromRoute]string type)
        {
            try
            {
                var file = Request.Form.Files[0]; // get file from angular
                var folderName = Path.Combine("UploadedFile", type);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = "Plant_Uploaded_" + DateTime.Now.ToString("hhmm_ddMMyyyy") + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                        file.CopyTo(stream);
                    return Ok(await Task.Run(() => System.Text.Json.JsonSerializer.Serialize(fullPath)));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw new COOException("Error: ", ex);
            }
        }
    }
}