using AuLacChamCong.DataApi;
using System;
using AuLacChamCong.Services.Modules.LichLamViec.Service;
using Microsoft.AspNetCore.Mvc;
using AuLacChamCong.Services.Modules.LichLamViec.Model;
using AuLacChamCong.Models.ViewModel;

namespace AuLacChamCong.Controllers
{
    public class LichLamViecController : ControllerBase
    {
        private readonly ILichLamViecServices _lichLamViecServices;
        public LichLamViecController(ILichLamViecServices lichLamViecServices)
        {
            _lichLamViecServices = lichLamViecServices;
        }
        [HttpGet("GetAllLichLamViec")]
        public async Task<IActionResult> GetAllLichLamViec()
        {
            var result = await _lichLamViecServices.GetAllLichLamViecsAsync();
            return Ok(result);
        }
        [HttpPost("CreateLichLamViec")]
        public async Task<IActionResult> CreateLichLamViec([FromBody]LichLamViecDb lichLamViec)
        {
            var result = await _lichLamViecServices.CreateLichLamViecAsync(lichLamViec);
            return Ok(result);
        }
        [HttpPost("UpdateLichLamViec")]
        public async Task<IActionResult> UpdateLichLamViec(LichLamViecDb lichLamViec)
        {
            var result = await _lichLamViecServices.UpdateLichLamViecAsync(lichLamViec);
            return Ok(result);
        }
        [HttpPost("DeleteLichLamViec")]
        public async Task<IActionResult> DeleteLichLamViec(int lichLamViecId)
        {
            var result = await _lichLamViecServices.DeleteLichLamViecAsync(lichLamViecId);
            return Ok(result);
        }
        [HttpGet("SearchLichLamViec")]
        public async Task<IActionResult> SearchLichLamViec(LichLamViecSearch lichLamViec)
        {
            var result = await _lichLamViecServices.Search(lichLamViec);
            return Ok(result);
        }
    }
}
