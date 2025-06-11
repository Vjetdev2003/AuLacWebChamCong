using AuLacChamCong.DataApi;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.LichLamViec.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuLacChamCong.Services.Modules.LichLamViec.Service
{
    public class LichLamViecService : ILichLamViecServices
    {
        private readonly ApplicationDbContext _dbContext;

        public LichLamViecService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateLichLamViecAsync(LichLamViecDb lichLamViec)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (lichLamViec == null)
                {
                    return "Dữ liệu lịch làm việc không hợp lệ!";
                }

                // Kiểm tra thời gian bắt đầu và kết thúc
                if (lichLamViec.ThoiGianBatDau >= lichLamViec.ThoiGianKetThuc)
                {
                    return "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc!";
                }

                // Kiểm tra xem lịch làm việc có trùng không (nếu cần)
                var existingSchedule = await _dbContext.LichLamViecDb
                    .FirstOrDefaultAsync(l => l.PsnPrkID == lichLamViec.PsnPrkID
                        && l.NgayTruc.Date == lichLamViec.NgayTruc.Date
                        && l.PhanLoai == lichLamViec.PhanLoai);
                if (existingSchedule != null)
                {
                    return $"Nhân viên đã có lịch làm việc trong ngày {lichLamViec.NgayTruc.ToShortDateString()} với phân loại {lichLamViec.PhanLoai}!";
                }

                // Thêm lịch làm việc vào cơ sở dữ liệu
                _dbContext.LichLamViecDb.Add(lichLamViec);
                await _dbContext.SaveChangesAsync();

                return "Tạo lịch làm việc thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi tạo lịch làm việc: {ex.Message}";
            }
        }

        public async Task<bool> DeleteLichLamViecAsync(int lichLamViecId)
        {
            try
            {
                var lichLamViec = await _dbContext.LichLamViecDb.FirstOrDefaultAsync(l => l.LichLamViecID == lichLamViecId);
                if (lichLamViec == null)
                {
                    return false;
                }

                _dbContext.LichLamViecDb.Remove(lichLamViec);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi xóa lịch làm việc: {ex.Message}");
                return false;
            }
        }

        public async Task<List<LichLamViecDb>> GetAllLichLamViecsAsync()
        {
            try
            {
                return await _dbContext.LichLamViecDb.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lấy danh sách lịch làm việc: {ex.Message}");
                return new List<LichLamViecDb>();
            }
        }

        public async Task<LichLamViecDb> GetLichLamViecByIdAsync(int lichLamViecId)
        {
            try
            {
                return await _dbContext.LichLamViecDb.FirstOrDefaultAsync(l => l.LichLamViecID == lichLamViecId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lấy lịch làm việc theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<List<LichLamViecViewModel>?> Search(LichLamViecSearch lichLamViecSearch)
        {
            try
            {
                // Tạo query cơ bản
                var query = from llv in _dbContext.LichLamViecDb
                            join p in _dbContext.Personnels on llv.PsnPrkID equals p.PsnPrkID into personnelGroup
                            from p in personnelGroup.DefaultIfEmpty()
                            select new LichLamViecViewModel
                            {
                                LichLamViecID = llv.LichLamViecID,
                                PsnPrkID = llv.PsnPrkID,
                                LoaiCongViecID = llv.LoaiCongViecID,
                                NoiDungChiTiet = llv.NoiDungChiTiet,
                                NgayTruc = llv.NgayTruc,
                                ThoiGianBatDau = llv.ThoiGianBatDau,
                                ThoiGianKetThuc = llv.ThoiGianKetThuc,
                                PhanLoai = llv.PhanLoai,
                                PsnName = p != null ? p.PsnName : "Không xác định"
                            };

                // Áp dụng điều kiện tìm kiếm
                // Tìm kiếm theo khoảng thời gian (From - To)
                if (lichLamViecSearch.From != default(DateTime) && lichLamViecSearch.To != default(DateTime))
                {
                    query = query.Where(l => l.NgayTruc.Date >= lichLamViecSearch.From.Date && l.NgayTruc.Date <= lichLamViecSearch.To.Date);
                }
                else if (lichLamViecSearch.From != default(DateTime))
                {
                    query = query.Where(l => l.NgayTruc.Date >= lichLamViecSearch.From.Date);
                }
                else if (lichLamViecSearch.To != default(DateTime))
                {
                    query = query.Where(l => l.NgayTruc.Date <= lichLamViecSearch.To.Date);
                }

                // Tìm kiếm theo phòng ban (DeptId)
                if (lichLamViecSearch.DeptId.IsNullOrEmpty())
                {
                    query = query.Where(l => _dbContext.Personnels
                        .Where(p => p.PsnPrkID == l.PsnPrkID && p.DeptPrkID == lichLamViecSearch.DeptId)
                        .Any());
                }

                // Tìm kiếm theo nhân viên (NhanVienId)
                if (lichLamViecSearch.NhanVienId > 0)
                {
                    query = query.Where(l => l.PsnPrkID == lichLamViecSearch.NhanVienId);
                }

                // Tìm kiếm theo loại công việc (LoaiCongViecId)
                if (lichLamViecSearch.LoaiCongViecId > 0)
                {
                    query = query.Where(l => l.LoaiCongViecID == lichLamViecSearch.LoaiCongViecId);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi tìm kiếm lịch làm việc: {ex.Message}");
                return null;
            }
        }
        public async Task<string> UpdateLichLamViecAsync(LichLamViecDb lichLamViec)
        {
            try
            {
                var existingLichLamViec = await _dbContext.LichLamViecDb
                    .FirstOrDefaultAsync(l => l.LichLamViecID == lichLamViec.LichLamViecID);
                if (existingLichLamViec == null)
                {
                    return $"Không tìm thấy lịch làm việc với ID: {lichLamViec.LichLamViecID}";
                }

                // Kiểm tra thời gian bắt đầu và kết thúc
                if (lichLamViec.ThoiGianBatDau >= lichLamViec.ThoiGianKetThuc)
                {
                    return "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc!";
                }

                // Cập nhật thông tin
                existingLichLamViec.PsnPrkID = lichLamViec.PsnPrkID;
                existingLichLamViec.LoaiCongViecID = lichLamViec.LoaiCongViecID;
                existingLichLamViec.NoiDungChiTiet = lichLamViec.NoiDungChiTiet;
                existingLichLamViec.NgayTruc = lichLamViec.NgayTruc;
                existingLichLamViec.ThoiGianBatDau = lichLamViec.ThoiGianBatDau;
                existingLichLamViec.ThoiGianKetThuc = lichLamViec.ThoiGianKetThuc;
                existingLichLamViec.PhanLoai = lichLamViec.PhanLoai;

                _dbContext.LichLamViecDb.Update(existingLichLamViec);
                await _dbContext.SaveChangesAsync();

                return "Cập nhật lịch làm việc thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi cập nhật lịch làm việc: {ex.Message}";
            }
        }
    }
}
