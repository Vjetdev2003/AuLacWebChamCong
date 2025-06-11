using AuLacChamCong.DataApi;
using AuLacChamCong.Services.LocationCompany.Model;
using AuLacChamCong.Services.Modules.BenhVien.Model;
using Microsoft.EntityFrameworkCore;

namespace AuLacChamCong.Services.Modules.BenhVien.Service
{
    public class HospitalServices : IHospitalServices
    {
        private readonly ApplicationDbContext _dbContext;
        public HospitalServices(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }
        public Task<int> CreateHospitalAsync(HospitalDb hospital)
        {
            throw new NotImplementedException();
        }

        public Task DeleteHospitalAsync(int hospitalId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<HospitalDb>> GetAllHospitalsAsync()
        {
           var result =  await _dbContext.HospitalDbs.ToListAsync();
            return result; 
        }

        public async Task<Coordinate> GetCoordinatesForNoiCongTac(string noiCongTac)
        {
            if (string.IsNullOrWhiteSpace(noiCongTac))
            {
                return new Coordinate { Latitude = 0, Longitude = 0 };
            }

            try
            {
                // Tìm bệnh viện trong cơ sở dữ liệu dựa trên NoiCongTac
                // Giả sử HospitalDb có các trường như Name hoặc Address để so khớp
                var hospital = await _dbContext.HospitalDbs
                .Where(h => h.HospitalName.Contains(noiCongTac) || (h.HospitalAddress != null && h.HospitalAddress.Contains(noiCongTac)))
                .FirstOrDefaultAsync();

                if (hospital != null && hospital.Latitude.HasValue && hospital.Longitude.HasValue)
                {
                    return new Coordinate
                    {
                        Latitude = (double)hospital.Latitude.Value,
                        Longitude = (double)hospital.Longitude.Value
                    };
                }

                // Nếu không tìm thấy trong cơ sở dữ liệu, trả về tọa độ mặc định
                // (Hoặc bạn có thể tích hợp API bên ngoài như Nominatim ở đây nếu cần)
                return new Coordinate { Latitude = 0, Longitude = 0 };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lấy tọa độ cho {noiCongTac}: {ex.Message}");
                return new Coordinate { Latitude = 0, Longitude = 0 };
            }
        }

        public Task<HospitalDb> GetHospitalByIdAsync(int hospitalId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateHospitalAsync(HospitalDb hospital)
        {
            throw new NotImplementedException();
        }
    }
}
