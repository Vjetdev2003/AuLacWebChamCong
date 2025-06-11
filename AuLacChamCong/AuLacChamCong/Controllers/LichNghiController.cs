using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.LichNghi.Model;
using AuLacChamCong.Services.Modules.LichNghi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LichNghiController : ControllerBase
    {
        private readonly ILichNghiServices _lichNghiServices;

        public LichNghiController(ILichNghiServices lichNghiServices)
        {
            _lichNghiServices = lichNghiServices;
        }

        [HttpGet("GetAllLichNghi")]
        public async Task<IActionResult> GetAllLichNghi()
        {
            var result = await _lichNghiServices.GetAllLichNghiAsync();
            return Ok(result);
        }
        [HttpPost("CreateDonXinNghi")]
        public async Task<IActionResult> CreateDonXinNghi(DonXinNghiViewModel request)
        {
            var result = await _lichNghiServices.CreateLichNghiAsync(request);
            return Ok(result);
        }
        [HttpGet("GetLichNghiByPsnPrkID/{psnPrkID}")]
        public async Task<IActionResult> GetLichNghiByPsnPrkID(int psnPrkID)
        {
            var result = await _lichNghiServices.GetLichNghiByPsnPrkID(psnPrkID);
            return Ok(result);
        }
        [HttpPost("UpdateLichNghiStatus")]
        public async Task<IActionResult> UpdateLichNghiStatus(int lichNghiID, int status)
        {
            var result = await _lichNghiServices.UpdateLichNghiStatusAsync(lichNghiID, status);
            return Ok(result);
        }
        [HttpGet("SearchLichNghi")]
        public async Task<IActionResult> SearchLichNghi(string? searchKeyword, int? status, string? loaiNghi)
        {
            var result = await _lichNghiServices.SearchLichNghiAsync(searchKeyword, status, loaiNghi);
            return Ok(result);
        }

        [HttpGet("GetHolidaysAsync")]
        public async Task<IActionResult> GetHolidaysByMonthAndYear(int month, int year)
        {
            var result = await _lichNghiServices.GetHolidaysAsync(month, year);
            return Ok(result);
        }

    }
}
