using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Shared;
using AuLacChamCong.Services.Modules.Profile.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuLacChamCong.Services.Modules.Profile.Service
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public UserProfileService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        public async Task<Personnels> GetUserProfileAsync(string psnPrkID)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();

                using (SqlCommand command = new SqlCommand("SP_GetProfile", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PsnPrkID", psnPrkID);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var user = new Personnels
                            {
                                PsnID = reader["PsnID"].ToString(),
                                PsnName = reader["PsnName"].ToString(),
                                PsnBirthday = reader["PsnBirthday"] as DateTime?,
                                PsnPlace = reader["PsnPlace"].ToString(),
                                PsnAddr = reader["PsnAddr"].ToString(),
                                PsnPhone = reader["PsnPhone"].ToString(),
                                ChuyenMon = reader["ChuyenMon"].ToString(),
                                DocJobRegNum = reader["DocJobRegNum"].ToString(),
                                PsnCardID = reader["PsnCardID"].ToString(),
                                Chucvu = reader["Chucvu"].ToString()
                            };

                            return user;
                        }
                        else
                        {
                            return null; // Or handle the case when no user is found
                        }
                    }
                }
            }
        }
    }
}
