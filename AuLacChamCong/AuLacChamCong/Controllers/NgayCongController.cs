using AuLacChamCong.Models.Shared;
using AuLacChamCong.Services.TinhCong;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NgayCongController : ControllerBase
    {
        private readonly ITinhCongService _tinhCongService;
        private readonly ILogger _logger;
        public NgayCongController(ITinhCongService tinhCongService)
        {
            _tinhCongService = tinhCongService;
        }

        [HttpGet("/getsKhoa")]
        public JsonResult getsKhoa()
        {
            var result = _tinhCongService.getsKhoa();
            return new JsonResult(result);
        }
        [HttpGet("/getsNhanVien")]
        public JsonResult getsNhanVien()
        {
            var result = _tinhCongService.getsNhanVien();
            return new JsonResult(result);
        }
        [HttpGet("/getsLoaiChamCong")]
        public JsonResult getsLoaiChamCong()
        {
            var result = _tinhCongService.getsLoaiChamCong();
            return new JsonResult(result);
        }
        [HttpGet("/getsLoaiNhanVien")]
        public JsonResult getsLoaiNhanVien()
        {
            var result = _tinhCongService.getTypeNhanVien();
            return new JsonResult(result);
        }
        [HttpPost("/addNhanVien")]
        public async Task<IActionResult> AddEmployee([FromBody] PersonnelsDb newEmployee)
        {
            if (ModelState.IsValid)
            {
                await _tinhCongService.AddEmployeeAsync(newEmployee);
                return Ok();
            }

            return BadRequest("Dữ liệu không hợp lệ");
        }
        [HttpGet("report/day")]
        public async Task<IActionResult> GetAttendanceReportByDay([FromQuery] DateTime? day)
        {
            var report = await _tinhCongService.GetAttendanceReportByDayAsync(day);
            return Ok(report);
        }

        // Lấy báo cáo chấm công theo tháng và năm
        [HttpGet("report/month")]
        public async Task<IActionResult> GetAttendanceReportByMonth([FromQuery] int? month, [FromQuery] int? year)
        {
            var report = await _tinhCongService.GetAttendanceReportByMonthAsync(month, year);
            return Ok(report);
        }
        [HttpGet("GetDepartments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _tinhCongService.GetDepartmentsAsync();
            return Ok(departments);
        }
        [HttpGet("BaoCaoChamCong")]
        public async Task<IActionResult> GetAllAttendanceReport()
        {
            var report = await _tinhCongService.GetAllAttendanceReportAsync();
            return Ok(report);
        }
        [HttpGet("GetTotalWorkHoursPerEmployee")]
        public async Task<IActionResult> GetTotalWorkHour([FromQuery] int? month, int? year)
        {
            try
            {
                var totalHours = await _tinhCongService.GetTotalWorkHoursPerEmployeeAsync(month, year);

                if (totalHours == null || !totalHours.Any())
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu chấm công cho tháng và năm đã chọn." });
                }

                var response = new WorkDaysResult
                {
                    TotalHours = totalHours,
                    Month = month?.ToString() ?? DateTime.Now.Month.ToString(),
                    Year = year?.ToString() ?? DateTime.Now.Year.ToString(),
                    TotalEmployees = totalHours.Count
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng giờ làm cho tháng {Month} năm {Year}", month, year);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy tổng giờ làm. Vui lòng thử lại sau." });
            }
        }
    }
}
