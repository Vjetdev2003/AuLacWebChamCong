using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuLacChamCong.Services.Modules.User.Service
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly IJSRuntime _jsRuntime;


        public TokenService(IConfiguration config, IJSRuntime jsRuntime)
        {
            _config = config;
            _jsRuntime = jsRuntime;
        }

        public string GenerateToken(string userId, string userName)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expireMinutes = Convert.ToInt32(jwtSettings["ExpireMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Chuyển từ JwtRegisteredClaimNames.Sub sang ClaimTypes.NameIdentifier
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),  // Đảm bảo userId là NameIdentifier
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // unique identifier
                new Claim("role", "User")  // role nếu cần
            };

            if (!string.IsNullOrEmpty(userName))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, userName));  // Nếu cần, lưu tên người dùng
            }

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task StoreToken(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }

        // Retrieve token from localStorage
        public async Task<string> GetToken()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        }

        // Remove token from localStorage (for logout)
        public async Task RemoveToken()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }
    }
}
