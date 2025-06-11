using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace AuLacChamCong.Services.Modules.NhanVien
{
    public class NhanVien : INhanVien
    {
        private readonly ApplicationDbContext _dbContext;
        public NhanVien(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<List<PersonnelsDb>> GetAllNhanVien()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateNhanVien(PersonnelsDb employee)
        {
            try
            {
                _dbContext.Personnels.Add(employee);
                var rows = await _dbContext.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating employee: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateNhanVien(PersonnelsDb employee)
        {
            try
            {
                var existingEmployee = await _dbContext.Personnels.FindAsync(employee.PsnPrkID);
                if (existingEmployee == null) return false;

                _dbContext.Entry(existingEmployee).CurrentValues.SetValues(employee);
                var rows = await _dbContext.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee: {ex.Message}");
                return false;
            }
        }
    }
}
