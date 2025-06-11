
using AuLacChamCong.Services.LocationCompany.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel;
using AuLacChamCong.Services.Modules.ChamCong.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChamCongController : ControllerBase
    {
        private readonly IChamCongService _chamCongService;
        private const double HospitalLatitude = 10.762622;  // Hospital latitude
        private const double HospitalLongitude = 106.660172; // Hospital longitude
        private const double MaxDistanceInMeters = 500; // Max allowed distance in meters
        public ChamCongController(IChamCongService chamCongService)
        {
            _chamCongService = chamCongService;
        }

        [HttpPost("CheckChamCong")]
        public JsonResult CheckChamCong(ChamCongHeader1 ChamCong)
        {
            var result = _chamCongService.CheckChamCong(ChamCong);
            return new JsonResult(result);
        }
        [HttpGet("GetChamCongHistory")]
        public async Task<IActionResult> GetChamCongHistory(decimal psnPrkID, int? month = null, int? year = null)
        {
            try
            {
                if (psnPrkID <= 0)
                {
                    return BadRequest("PsnPrkID phải lớn hơn 0.");
                }

                var result = await _chamCongService.GetChamCongHistory(psnPrkID, month, year);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
        [HttpPost("CheckIn")]
        public async Task<IActionResult> CheckIn([FromBody] ChamCongDetailModel model)
        {
            var result = await _chamCongService.CheckInAsync(model.ChamCongLines,model.ChamCongHeader);
            if (result.Contains("thành công"))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("CheckOut")]
        public async Task<IActionResult>CheckOut([FromBody] ChamCongLineModel chamCongLine)
        {
            var result = await _chamCongService.CheckOutAsync(chamCongLine);
            if (result.Contains("thành công"))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("CheckLocation")]
        public async Task<IActionResult> CheckInLocation([FromBody] Coordinate locationRequest)
        {
            try
            {
                // Calculate the distance from the hospital to the user's location
                var distance = CalculateDistance(locationRequest.Latitude, locationRequest.Longitude, HospitalLatitude, HospitalLongitude);

                // Check if the user is within the allowed distance
                if (distance > MaxDistanceInMeters)
                {
                    return Ok(new { success = false, message = "Vui lòng di chuyển đến gần bệnh viện để chấm công!" });
                }

                // If within range, allow check-in
                return Ok(new { success = true, message = "Chấm công thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }
        [HttpPost("ChamCongTuXa")]
        public async Task<IActionResult> ChamCongTuXa([FromBody] ChamCongDetailModel request)
        {
            var result = await _chamCongService.ChamCongTuXa(request.ChamCongLines, request.ChamCongHeader);
            if (result.Contains("thành công"))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // Method to calculate distance between two points (in meters)
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Radius of the Earth in meters
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Result in meters
        }
    }
}
