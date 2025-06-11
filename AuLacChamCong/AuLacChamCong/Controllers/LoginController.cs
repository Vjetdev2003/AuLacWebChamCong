using AuLacChamCong.Services.Modules.User.Model;
using AuLacChamCong.Services.Modules.User.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Tags("login")]
    public class LoginController : ControllerBase
    {
        private readonly Login_Services _loginServices;


        public LoginController(
             Login_Services loginServices)
        {
            _loginServices = loginServices;
        }
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromForm] LoginRequest request)
        {
            try
            {
                var result = await _loginServices.Login(request.UserID, request.UserPassword);

                if (result != null)
                {
                    return Ok(new { success = true, message = "Đăng nhập thành công!.", data = result });

                }
                else
                {
                    return NotFound(new { success = false, message = "Sai tài khoản đăng nhập hoặc mật khẩu! Vui lòng thử lại sau." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                return Ok(new { success = true, message = "Đăng xuất thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server khi đăng xuất: {ex.Message}" });
            }
        }
    }
}
