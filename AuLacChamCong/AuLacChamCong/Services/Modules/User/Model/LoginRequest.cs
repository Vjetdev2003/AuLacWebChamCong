using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Services.Modules.User.Model
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Tài Khoản là bắt buộc.")]
        public string UserID { get; set; }
        public string? UserPassword { get; set; }
    }
}
