using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.ChamCong.Services.FaceIdSerivces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FaceIdController : ControllerBase
    {
        private readonly IFaceIdService _faceIdService;
        public FaceIdController(IFaceIdService faceIdService)
        {
            _faceIdService = faceIdService;
        }
        [HttpPost("DetectFace")]
        public async Task<IActionResult> DetectFace(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new { Message = "Hình ảnh không hợp lệ!" });
            }

            try
            {
                // Chuyển đổi IFormFile thành byte[]
                using var stream = new MemoryStream();
                await imageFile.CopyToAsync(stream);
                byte[] faceImage = stream.ToArray();

                // Gọi service để phát hiện khuôn mặt
                bool hasFace = await _faceIdService.DetectFace(faceImage);

                // Trả về phản hồi với định dạng mong đợi (HasFace dưới dạng chuỗi "true" hoặc "false")
                return Ok(new { HasFace = hasFace.ToString().ToLower() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi phát hiện khuôn mặt: {ex.Message}");
                return StatusCode(500, new { Message = "Lỗi khi phát hiện khuôn mặt: " + ex.Message });
            }
        }

        [HttpPost("RegisterFace")]
        public async Task<IActionResult> RegisterFace([FromBody] RegisterFaceRequest request)
        {
            if (request == null || request.PsnPrkID <= 0 || request.EncodedImage == null || !request.EncodedImage.Any())
            {
                Console.WriteLine("[ERROR] Yêu cầu không hợp lệ tại controller RegisterFace");
                return BadRequest(new { Message = "Yêu cầu không hợp lệ! Kiểm tra PsnPrkID hoặc danh sách hình ảnh." });
            }

            try
            {
                Console.WriteLine($"[INFO] Nhận yêu cầu RegisterFace với PsnPrkID: {request.PsnPrkID}, Số lượng hình ảnh: {request.EncodedImage.Count}");
                string result = await _faceIdService.RegisterFace(request.PsnPrkID, request.EncodedImage);
                return Ok(new { Message = result }); // Sử dụng "Message" (chữ hoa)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi tại controller RegisterFace: {ex.Message}");
                return StatusCode(500, new { Message = "Lỗi khi đăng ký khuôn mặt: " + ex.Message }); // Sử dụng "Message" (chữ hoa)
            }
        }
        public class VerifyFaceRequest
        {
            public decimal PsnPrkID { get; set; }
            public IFormFile ImageFile { get; set; }
        }

        [HttpPost("VerifyFace")]
        public async Task<IActionResult> VerifyFace([FromForm] VerifyFaceRequest request)
        {
            if (request.PsnPrkID <= 0)
            {
                return BadRequest(new { Message = "ID người dùng không hợp lệ!" });
            }

            if (request.ImageFile == null || request.ImageFile.Length == 0)
            {
                return BadRequest(new { Message = "Hình ảnh không hợp lệ!" });
            }

            try
            {
                // Chuyển đổi IFormFile thành byte[]
                using var stream = new MemoryStream();
                await request.ImageFile.CopyToAsync(stream);
                byte[] faceImage = stream.ToArray();

                // Gọi service để xác minh khuôn mặt
                bool isMatch = await _faceIdService.VerifyFace(request.PsnPrkID, faceImage);

                // Trả về phản hồi với định dạng mong đợi
                return Ok(new { IsMatch = isMatch.ToString().ToLower() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi xử lý hình ảnh trong VerifyFace: {ex.Message}");
                return StatusCode(500, new { Message = "Lỗi khi xử lý hình ảnh: " + ex.Message });
            }
        }

        [HttpGet("GetAllFaceId")]
        public async Task<IActionResult> GetAllFaceId()
        {
            try
            {
                var faceIds = await _faceIdService.GetAllFaceId();
                return Ok(faceIds);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lấy danh sách FaceId: {ex.Message}");
                return StatusCode(500, new { Message = "Lỗi khi lấy danh sách FaceId: " + ex.Message });
            }
        }
    }
}
