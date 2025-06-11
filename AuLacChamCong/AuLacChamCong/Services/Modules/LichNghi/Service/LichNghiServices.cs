using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Enum;
using AuLacChamCong.Models.Shared.ModelSearch;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.LichNghi.Model;
using Microsoft.EntityFrameworkCore;

namespace AuLacChamCong.Services.Modules.LichNghi.Service
{
    public class LichNghiServices : ILichNghiServices
    {
        private readonly ApplicationDbContext _dbContext;
        public LichNghiServices(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateLichNghiAsync(DonXinNghiViewModel request)
        {
            try
            {
                // Validate input
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");
                }

                // Validate LoaiNghiID and map to LoaiNghiName
                if (!Enum.IsDefined(typeof(LoaiNghiType), request.LoaiNghiID))
                {
                    throw new ArgumentException($"Invalid LoaiNghiID: {request.LoaiNghiID}");
                }

                var loaiNghiType = (LoaiNghiType)request.LoaiNghiID;
                string loaiNghiName = loaiNghiType switch
                {
                    LoaiNghiType.NghiPhep => "Nghỉ phép",
                    LoaiNghiType.NghiOm => "Nghỉ ốm",
                    LoaiNghiType.NghiThaiSan => "Nghỉ thai sản",
                    LoaiNghiType.NghiKetHon => "Nghỉ kết hôn",
                    LoaiNghiType.NghiKhongPhep => "Nghỉ không phép",
                    LoaiNghiType.NghiDaoTao => "Nghỉ đào tạo",
                    LoaiNghiType.NghiLe => "Nghỉ lễ",
                    _ => throw new InvalidOperationException("Unknown leave type")
                };
                var status = (Status)request.Status;
                string StatusDescription = status switch
                {
                    Status.Pending => "Pending",
                    Status.Accpect => "Accpected",
                    Status.Reject => "Rejected",
                };
                // Map ViewModel to Db entity
                var lichNghi = new LichNghiDb
                {
                    PsnPrkID = request.PsnPrkID,
                    LoaiNghiID = request.LoaiNghiID,
                    LoaiNghiName = loaiNghiName,
                    ThoiGianBatDau = request.ThoiGianBatDau,
                    ThoiGianKetThuc = request.ThoiGianKetThuc,
                    LyDo = request.LyDo,
                    Status = 0,
                    StatusDescription = StatusDescription,
                    NgayNopDon = DateTime.Now,
                  //  CaLamViec = request.CaLamViec
                };

                // Add to DbContext and save
                await _dbContext.LichNghiDbs.AddAsync(lichNghi);
                var row = await _dbContext.SaveChangesAsync();
                return row > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating LichNghi: {ex.Message}");
                return false;
            }
        }

        public async Task<List<LichNghiDb>> GetAllLichNghiAsync()
        {
            try
            {
                // Fetch all leave records
                var lichNghiList = await _dbContext.LichNghiDbs
                    .Where(l=>l.Status == 0)
                    .Include(l => l.Personnels)
                    .ToListAsync();

                return lichNghiList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all LichNghi: {ex.Message}");
                return new List<LichNghiDb>();
            }
        }

        public async Task<List<DonXinNghiViewModel>> GetLichNghiByPsnPrkID(int psnPrkID)
        {
            try
            {
                // Fetch leave history for a specific PsnPrkID
                var lichNghiList = await _dbContext.LichNghiDbs
                    .Where(ln => ln.PsnPrkID == psnPrkID)
                    .Include(d => d.Personnels)
                    .OrderByDescending(ln => ln.NgayNopDon) // Order by submission date
                    .ToListAsync();

                // Ánh xạ từ LichNghiDb sang DonXinNghiViewModel
  
                var result = lichNghiList.Select(ln => new DonXinNghiViewModel
                {
                    LichNghiID = ln.LichNghiID,
                    PsnPrkID = ln.PsnPrkID,
                    LoaiNghiID = ln.LoaiNghiID ?? 0,
                    ThoiGianBatDau = ln.ThoiGianBatDau,
                    ThoiGianKetThuc = ln.ThoiGianKetThuc,
                    LyDo = ln.LyDo,
                    Status = ln.Status,
                    NgayNopDon = ln.NgayNopDon,
                    LoaiNghiName = ln.LoaiNghiName, // Nếu có
                    StatusDescription = Enum.GetName(typeof(Status), ln.Status) ?? "Không xác định",
                    //CaLamViec = ln.CaLamViec
                }).ToList();

                return result ?? new List<DonXinNghiViewModel>(); // Trả về danh sách rỗng nếu null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching LichNghi history for PsnPrkID {psnPrkID}: {ex.Message}");
                return new List<DonXinNghiViewModel>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        public Task<List<LichNghiDb>> GetLichNghiHistory()
        {
            var query = _dbContext.LichNghiDbs.AsQueryable();

            var result = query.Where(l => l.Status == 1 || l.Status == -1)
                .Include(l=>l.Personnels)
                .OrderByDescending(l => l.NgayNopDon)
                .ToListAsync();
            return result;
        }

        public async Task<List<LichNghiSearch>> SearchLichNghiAsync(string? searchKeyword, int? status, string? loaiNghi)
        {
            var query = _dbContext.LichNghiDbs.AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(d => d.Personnels.PsnName.Contains(searchKeyword));
            }
            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status);
            }
            if (!string.IsNullOrEmpty(loaiNghi))
            {
                query = query.Where(e => e.LoaiNghiName.Contains(loaiNghi));
            }
            var result = await query
               .Include(d => d.Personnels)
               .Select(d => new LichNghiSearch
               {
                   LichNghiID = d.LichNghiID,
                   PsnPrkID = d.PsnPrkID,
                   LoaiNghiName = d.LoaiNghiName,
                   NgayNopDon = d.NgayNopDon,
                   ThoiGianBatDau = d.ThoiGianBatDau,
                   ThoiGianKetThuc = d.ThoiGianKetThuc,
                   LyDo = d.LyDo,
                   Status = d.Status,
                   StatusDescription = d.StatusDescription,
                   PersonnelsName = d.Personnels.PsnName
                   // Add more properties from DonCongTacDb as needed
               })
               .ToListAsync();

            return result;
        }

        public async Task<bool> UpdateLichNghiStatusAsync(int lichNghiID, int status)
        {
            try
            {
                // Find the leave record by ID
                var lichNghi = await _dbContext.LichNghiDbs
                    .FirstOrDefaultAsync(ln => ln.LichNghiID == lichNghiID);

                if (lichNghi == null)
                {
                    Console.WriteLine($"LichNghi with ID {lichNghiID} not found.");
                    return false;
                }

                // Update status and description
                lichNghi.Status = status;
                lichNghi.StatusDescription = status switch
                {
                    0 => "Pending",
                    1 => "Approved",
                    2 => "Rejected",
                    _ => "Unknown"
                };

                // Save changes
                int rowsAffected = await _dbContext.SaveChangesAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating LichNghi status for ID {lichNghiID}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<NghiLeDb>> GetHolidaysAsync(int? month, int? year)
        {
            try
            {
                // Query holidays from NghiLeDb table
                var holidays = await _dbContext.NghiLeDbs
                    .Where(h => h.Year == year && h.HolidayDate.Month == month)
                    .OrderBy(h => h.HolidayDate) // Sắp xếp theo ngày
                    .ToListAsync();

                return holidays ?? new List<NghiLeDb>(); // Trả về danh sách rỗng nếu không có dữ liệu
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching holidays for month {month} and year {year}: {ex.Message}");
                return new List<NghiLeDb>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }
        public async Task AddHolidayAsync(NghiLeDb holiday)
        {
            try
            {
                _dbContext.NghiLeDbs.Add(holiday);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding holiday: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateHolidayAsync(NghiLeDb holiday)
        {
            try
            {
                var existingHoliday = await _dbContext.NghiLeDbs.FindAsync(holiday.HolidayId);
                if (existingHoliday != null)
                {
                    existingHoliday.HolidayDate = holiday.HolidayDate;
                    existingHoliday.HolidayName = holiday.HolidayName;
                    existingHoliday.Description = holiday.Description;
                    existingHoliday.Year = holiday.Year;
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating holiday: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteHolidayAsync(int holidayId)
        {
            try
            {
                var holiday = await _dbContext.NghiLeDbs.FindAsync(holidayId);
                if (holiday != null)
                {
                    _dbContext.NghiLeDbs.Remove(holiday);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error deleting holiday: {ex.Message}");
                throw;
            }
        }
    }
}
