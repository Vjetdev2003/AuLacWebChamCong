using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuLacChamCong.Services.Modules.User.Service
{
    public class Login_Services
    {
        private readonly ApiContext _context;
        private readonly ILogger<Login_Services> _logger;
        private readonly TokenService _tokenService;

        public Login_Services(ApiContext context, ILogger<Login_Services> logger, TokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
        }
        public async Task<object?> Login(string UserID, string UserPassword)
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SP_GetLoginInfo";
                        command.CommandType = CommandType.StoredProcedure;

                        string encodedPassword = EncryptionHelper.EncodeString(UserPassword);
                        _logger.LogInformation($"Encoded Password: {encodedPassword}");

                        command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.NVarChar, 30) { Value = UserID });
                        command.Parameters.Add(new SqlParameter("@UserPassword", SqlDbType.NVarChar, 200) { Value = encodedPassword });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Lấy thông tin người dùng từ query
                                var user = new
                                {
                                    UserPrkID = reader.IsDBNull(0) ? null : reader[0].ToString(),
                                    UserID = reader.IsDBNull(1) ? null : reader[1].ToString(),
                                    PsnPrkID = reader.IsDBNull(2) ? null : reader[2].ToString(),
                                    PsnName = reader.IsDBNull(3) ? null : reader[3].ToString(),
                                    PsnTypeID = reader.IsDBNull(4) ? null : reader[4].ToString(),
                                    IsFullRightsHSBADT = reader.IsDBNull(5) ? "0" : reader[5].ToString(),
                                    PsnTypeName = reader.IsDBNull(6) ? null : reader[6].ToString(),
                                    DeptPrkID = reader.IsDBNull(7) ? null : reader[7].ToString(),
                                    DeptName = reader.IsDBNull(8) ? null : reader[8].ToString(),
                                    HspID = reader.IsDBNull(9) ? null : reader[9].ToString(),
                                    HspName = reader.IsDBNull(10) ? null : reader[10].ToString()
                                };

                                // Tạo token
                                string token = _tokenService.GenerateToken(user.UserID, user.PsnName);

                                // Trả về đối tượng chứa user và token
                                return new { User = user, Token = token };
                            }
                        }
                    }
                }
                return null; // Trả về null nếu không tìm thấy người dùng
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong Login_Services: {ex.Message}", ex);
                throw;
            }
        }
    }
}
