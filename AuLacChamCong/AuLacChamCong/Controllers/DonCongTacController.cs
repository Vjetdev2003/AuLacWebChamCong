using AuLacChamCong.DataApi;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.BenhVien.Service;
using AuLacChamCong.Services.Modules.DonCongTac.Model;
using AuLacChamCong.Services.Modules.DonCongTac.Service;
using AuLacChamCong.Services.Modules.Notification.Model;
using AuLacChamCong.Services.Modules.Notification.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DonCongTacController : ControllerBase
    {
        private readonly IDonCongTacServices _donCongTacSevices;
        private readonly INotificationServices _notificationServices;
        private readonly IHospitalServices _hospitalServices;
        private readonly ApplicationDbContext _dbContext;
        public DonCongTacController(IDonCongTacServices donCongTacSevices, INotificationServices notificationServices, ApplicationDbContext dbContext,IHospitalServices hospitalServices)
        {
            _donCongTacSevices = donCongTacSevices;
            _notificationServices = notificationServices;
            _hospitalServices = hospitalServices;
            _dbContext = dbContext;
        }

        [HttpPost("CreateDonCongTac")]
        public async Task<IActionResult> CreateDonCongTac(DonCongTacViewModel donCongTac)
        {
            try
            {
                if (donCongTac == null)
                {
                    return BadRequest("Đơn công tác không được để trống.");
                }

                var emailSent = await _donCongTacSevices.CreateDonCongTacAsync(donCongTac);

                if (emailSent)
                {
                    return Ok(new { success = true, message = "Tạo phiếu yêu cầu đi công tác thành công." });
                }
                else
                {
                    return Ok(new { success = true, message = "Phiếu công tác đã được tạo, nhưng gửi email xác nhận thất bại." });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }
        [HttpGet("GetAllDonCongTac")]
        public async Task<IActionResult> GetAllDonCongTac()
        {
            try
            {
                var result = await _donCongTacSevices.GetAllDonCongTacAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }

        [HttpGet("GetHistoryForm")]
        public async Task<IActionResult> GetHistoryForm(int psnPrkID)
        {
            try
            {
                // Lấy lịch sử đơn công tác cho PsnPrkID
                var result = await _donCongTacSevices.GetHistoryDonCongTacByPsnPrkIDAsync(psnPrkID);

                if (result == null || !result.Any())
                {
                    return NotFound(new { success = false, message = "Không tìm thấy lịch sử cho nhân viên này." });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }

        [HttpPut("UpdateDonCongTacStatus/{mngDonCongTacPrkID}")]
        public async Task<IActionResult> UpdateDonCongTacStatus(int mngDonCongTacPrkID, int status)
        {
            try
            {
                var result = await _donCongTacSevices.UpdateDonCongTacStatusAsync(mngDonCongTacPrkID, status);
                var donCongTacDb = await _dbContext.DonCongTacs.SingleOrDefaultAsync(d => d.MngDonCongTacPrkID == mngDonCongTacPrkID);
                var notification = new NotificationDb
                {
                    Title = "Cập nhật trạng thái đơn công tác",
                    Message = $"Đơn công tác {mngDonCongTacPrkID} đã được cập nhật trạng thái thành công.",
                    PsnPrkID = donCongTacDb.PsnPrkID
                };
                await _notificationServices.CreateNotificationAsync(notification);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn công tác." });
                }
                return Ok(new { success = true, message = "Cập nhật trạng thái thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }
        [HttpGet("Search")]
        public async Task<IActionResult> SearchDonCongTac([FromQuery] string keyword,[FromQuery] int? status, [FromQuery] string? noiCongTac)
        {
            try
            {
                var result = await _donCongTacSevices.SearchDonCongTacAsync(keyword,status,noiCongTac);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }
        [HttpGet("Hospital/GetAllHospitals")]
        public async Task<IActionResult> GetAllHospitals()
        {
            var result = await _hospitalServices.GetAllHospitalsAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { success = false, message = "Không tìm thấy bệnh viện." });
            }
            return Ok(result);
        }
        [HttpGet("Hospital/GetCoordinatesForNoiCongTac/{noiCongTac}")]
        public async Task<IActionResult> GetCoordinateForNoiCongTac(string noiCongTac)
        {
            try
            {
                var result = await _hospitalServices.GetCoordinatesForNoiCongTac(noiCongTac);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });

            }
        }
    }
}
