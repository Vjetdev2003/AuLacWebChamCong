using AuLacChamCong.DataApi;
using AuLacChamCong.Services.Modules.ChamCong.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AuLacChamCong.Services.Modules.ChamCong.Services
{
    public class ChamCongService : IChamCongService
    {
        private readonly ApplicationDbContext _dbContext;
        private const double HospitalLatitude = 10.762622;  // Example hospital latitude
        private const double HospitalLongitude = 106.660172; // Example hospital longitude
        private const double MaxDistanceInMeters = 500;
        public ChamCongService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public decimal CheckChamCong(ChamCongHeader1 chamCongHeader)
        {
            var ChamCongHeader = _dbContext.ChamCongHeader1s.Where(e => e.NamCC == chamCongHeader.NamCC && e.PsnPrkID == chamCongHeader.PsnPrkID && e.ThangCC == chamCongHeader.ThangCC && e.NgayCC == chamCongHeader.NgayCC);
            if (chamCongHeader == null)
            {
                var id = _dbContext.ChamCongHeader1s.Add(chamCongHeader);
                _dbContext.SaveChanges();
                return id.Entity.MngChamCongPrkID;
            }
            return ChamCongHeader.FirstOrDefault().MngChamCongPrkID;
        }

        public string createChamCong(ChamCongLine1 chamCongLine)
        {
            if (chamCongLine == null)
            {
                return "ChamCongLine is null";
            }
            _dbContext.ChamCongLine1.Add(chamCongLine);
            if (_dbContext.SaveChanges() > 0)
            {
                return "Success";
            }
            return "false";
        }

        public string deleteChamCong(ChamCongLine1 chamCongLine)
        {
            _dbContext.ChamCongLine1.Remove(chamCongLine);
            if (_dbContext.SaveChanges() > 0)
            {
                return "Success";
            }
            return "false";
        }

        public string updateChamCong(ChamCongLine1 chamCongLine)
        {
            _dbContext.ChamCongLine1.Update(chamCongLine);
            if (_dbContext.SaveChanges() > 0)
            {
                return "Success";
            }
            return "false";
        }

        public async Task<List<HistoryResponse>> GetChamCongHistory(decimal psnPrkID, int? month = null, int? year = null)
        {
            // Truy vấn chính để lấy dữ liệu thô từ DB
            var query = from header in _dbContext.ChamCongHeader1s
                        join line in _dbContext.ChamCongLine1
                        on new { header.PsnPrkID, header.NgayCC, header.ThangCC, header.NamCC }
                        equals new
                        {
                            PsnPrkID = (decimal)line.UserWritePrkID,
                            NgayCC = line.NgayCham.Day, // Truy cập trực tiếp
                            ThangCC = line.NgayCham.Month,
                            NamCC = line.NgayCham.Year
                        }
                        where header.PsnPrkID == psnPrkID
                        select new { header, line };

            if (month.HasValue)
            {
                query = query.Where(h => h.header.ThangCC == month.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(h => h.header.NamCC == year.Value);
            }

            // Lấy dữ liệu thô vào bộ nhớ
            var rawData = await query.ToListAsync();

            // Nhóm và ánh xạ dữ liệu trong bộ nhớ
            var history = rawData
                .GroupBy(h => h.header) // Nhóm theo header
                .Select(g => new HistoryResponse
                {
                    MngChamCongPrkID = g.Key.MngChamCongPrkID,
                    PsnPrkID = g.Key.PsnPrkID,
                    ThangCC = g.Key.ThangCC,
                    NamCC = g.Key.NamCC,
                    NgayCC = g.Key.NgayCC,
                    Lines = g
                        .Select(l => new ChamCongLineModel
                        {
                            MngChamCongPrkID = l.line.MngChamCongPrkID,
                            Buoi = l.line.Buoi,
                            ChamCong = l.line.ChamCong,
                            NgayCham = l.line.NgayCham,
                            NgayChinhSua = l.line.NgayChinhSua,
                            GioBatDau = l.line.GioBatDau,
                            GioKetThuc = l.line.GioKetThuc,
                            UserWritePrkID = l.line.UserWritePrkID,
                            ComputerIP = l.line.ComputerIP,
                            ComputerName = l.line.ComputerName,
                            Status = l.line.Status,
                        })
                        .GroupBy(l => new
                        {
                            l.NgayCham,
                            l.Buoi,
                            l.GioBatDau,
                            l.GioKetThuc
                        }) // Nhóm để lọc trùng lặp (nếu có)
                        .Select(grp => grp.First()) // Chỉ lấy bản ghi đầu tiên nếu trùng
                        .OrderBy(l => l.Buoi)
                        .ThenBy(l => l.GioBatDau)
                        .ToList()
                })
                .ToList();

            // Loại bỏ các HistoryResponse không có Lines
            history = history.Where(h => h.Lines.Any()).ToList();

            return history ?? new List<HistoryResponse>();
        }
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3;
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        public async Task<string> CheckInAsync(ChamCongLineModel chamCongLineModel, ChamCongHeaderModel chamCongHeaderModel)
        {
            try
            {
                if (chamCongLineModel == null || chamCongHeaderModel == null)
                {
                    return "Dữ liệu đầu vào không hợp lệ!";
                }

                // Kiểm tra và tạo/reuse header
                var existingHeader = await _dbContext.ChamCongHeader1s
                    .FirstOrDefaultAsync(h => h.PsnPrkID == chamCongHeaderModel.PsnPrkID &&
                                              h.NgayCC == chamCongHeaderModel.NgayCC &&
                                              h.ThangCC == chamCongHeaderModel.ThangCC &&
                                              h.NamCC == chamCongHeaderModel.NamCC);

                ChamCongHeader1 chamCongHeader1;
                if (existingHeader == null)
                {
                    chamCongHeader1 = new ChamCongHeader1
                    {
                        PsnPrkID = chamCongHeaderModel.PsnPrkID,
                        ThangCC = chamCongHeaderModel.ThangCC,
                        NamCC = chamCongHeaderModel.NamCC,
                        NgayCC = chamCongHeaderModel.NgayCC
                    };
                    _dbContext.ChamCongHeader1s.Add(chamCongHeader1);
                }
                else
                {
                    chamCongHeader1 = existingHeader;
                }

                // Tạo bản ghi line mà không gán MngChamCongPrkID từ header
                TimeSpan gioBatDau = chamCongLineModel.GioBatDau.Value;
                chamCongLineModel.Buoi = gioBatDau.Hours < 12 ? 1 : 2;
                chamCongLineModel.ChamCong = 1;

                var chamCongLine1 = new ChamCongLine1
                {
                    Buoi = chamCongLineModel.Buoi,
                    ChamCong = chamCongLineModel.ChamCong,
                    NgayCham = chamCongLineModel.NgayCham,
                    NgayChinhSua = DateTime.Now,
                    GioBatDau = chamCongLineModel.GioBatDau,
                    GioKetThuc = chamCongLineModel.GioKetThuc,
                    UserWritePrkID = chamCongLineModel.UserWritePrkID,
                    ComputerIP = chamCongLineModel.ComputerIP,
                    ComputerName = chamCongLineModel.ComputerName,
                    Status = chamCongLineModel.Status ?? 0,
                    StatusDescription = "Chấm công tại cơ quan"
                    // Không gán MngChamCongPrkID, để database tự sinh
                };

                _dbContext.ChamCongLine1.Add(chamCongLine1);
                await _dbContext.SaveChangesAsync();

                return "Check-In thành công!";
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi khi thực hiện Check-In: {ex.Message}";
            }
        }

        public async Task<string> CheckOutAsync(ChamCongLineModel request)
        {
            try
            {
                Console.WriteLine($"[DEBUG] CheckOutAsync: Request={JsonConvert.SerializeObject(request)}");

                if (request == null || request.UserWritePrkID <= 0 || request.Buoi <= 0 || request.NgayCham == default)
                {
                    Console.WriteLine("[ERROR] Dữ liệu đầu vào không hợp lệ!");
                    return "Dữ liệu đầu vào không hợp lệ!";
                }

                var userId = request.UserWritePrkID; // Giữ nguyên decimal, không chuyển sang int
                Console.WriteLine($"[DEBUG] UserId={userId}, Buoi={request.Buoi}, NgayCham={request.NgayCham:yyyy-MM-dd}");

                // Tìm bản ghi chưa Check-Out gần nhất (ChamCong == 1) trong cùng User, Buoi, NgayCham
                var chamCongLine = await _dbContext.ChamCongLine1
                    .Where(c => c.UserWritePrkID == userId
                             && c.Buoi == request.Buoi
                             && c.NgayCham.Date == request.NgayCham.Date
                             && c.ChamCong == 1)
                    .OrderByDescending(c => c.NgayChinhSua) // Lấy bản ghi Check-In gần nhất dựa trên thời gian chỉnh sửa
                    .FirstOrDefaultAsync();

                if (chamCongLine == null)
                {
                    Console.WriteLine("[DEBUG] Không tìm thấy bản ghi Check-In chưa Check-Out.");
                    return "Không tìm thấy bản ghi Check-In chưa Check-Out. Bạn cần thực hiện Check-In trước.";
                }

                Console.WriteLine($"[DEBUG] Tìm thấy bản ghi: GioBatDau={chamCongLine.GioBatDau}, ChamCong={chamCongLine.ChamCong}");

                // Cập nhật GioKetThuc
                chamCongLine.GioKetThuc = request.GioKetThuc ?? DateTime.Now.TimeOfDay; // Dùng giờ hiện tại nếu null
                chamCongLine.NgayChinhSua = DateTime.Now;
                chamCongLine.ChamCong = 2;

                // Tính thời gian làm việc
                TimeSpan workTime = TimeSpan.Zero;
                if (chamCongLine.GioBatDau != TimeSpan.Zero && chamCongLine.GioKetThuc != null && chamCongLine.GioKetThuc > chamCongLine.GioBatDau)
                {
                    workTime = chamCongLine.GioKetThuc.Value - chamCongLine.GioBatDau.Value;
                }

                const double requiredHoursPerBuoi = 4.0;
                chamCongLine.Status = workTime.TotalHours >= requiredHoursPerBuoi ? 1 : 2; // 1: Đủ giờ, 2: Thiếu giờ

                Console.WriteLine($"[DEBUG] Cập nhật: GioKetThuc={chamCongLine.GioKetThuc}, Status={chamCongLine.Status}");
                _dbContext.ChamCongLine1.Update(chamCongLine);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("[DEBUG] Lưu DB thành công.");

                return "Check-Out thành công!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi thực hiện Check-Out: {ex.Message}, StackTrace: {ex.StackTrace}");
                return $"Đã xảy ra lỗi khi thực hiện Check-Out: {ex.Message}";
            }
        }

        public async Task<string> ChamCongTuXa(ChamCongLineModel chamCongLine, ChamCongHeaderModel chamCongHeader)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (chamCongLine == null || chamCongHeader == null)
                {
                    return "Dữ liệu đầu vào không hợp lệ!";
                }

                if (chamCongLine.UserWritePrkID <= 0 || chamCongLine.NgayCham == default)
                {
                    return "Thông tin người dùng hoặc ngày chấm công không hợp lệ!";
                }

                // Xác định buổi (sáng/chiều) dựa trên giờ bắt đầu
                TimeSpan gioBatDau = chamCongLine.GioBatDau ?? DateTime.Now.TimeOfDay;
                chamCongLine.Buoi = gioBatDau.Hours < 12 ? 1 : 2;

                // Kiểm tra trạng thái Check-In/Check-Out
                var existingChamCongLine = await _dbContext.ChamCongLine1
                    .Where(c => c.UserWritePrkID == chamCongLine.UserWritePrkID
                             && c.Buoi == chamCongLine.Buoi
                             && c.NgayCham.Date == chamCongLine.NgayCham.Date
                             && c.ChamCong == 1) // Tìm bản ghi đã Check-In nhưng chưa Check-Out
                    .OrderByDescending(c => c.NgayChinhSua)
                    .FirstOrDefaultAsync();

                if (existingChamCongLine != null)
                {
                    // Đã có Check-In, thực hiện Check-Out
                    existingChamCongLine.GioKetThuc = chamCongLine.GioKetThuc ?? DateTime.Now.TimeOfDay;
                    existingChamCongLine.NgayChinhSua = DateTime.Now;
                    existingChamCongLine.ChamCong = 2; // Đánh dấu đã Check-Out

                    // Tính thời gian làm việc
                    TimeSpan workTime = TimeSpan.Zero;
                    if (existingChamCongLine.GioBatDau != TimeSpan.Zero && existingChamCongLine.GioKetThuc > existingChamCongLine.GioBatDau)
                    {
                        workTime = existingChamCongLine.GioKetThuc.Value - existingChamCongLine.GioBatDau.Value;
                    }

                    const double requiredHoursPerBuoi = 4.0;
                    existingChamCongLine.Status = workTime.TotalHours >= requiredHoursPerBuoi ? 1 : 2; // 1: Đủ giờ, 2: Thiếu giờ

                    _dbContext.ChamCongLine1.Update(existingChamCongLine);
                    await _dbContext.SaveChangesAsync();

                    return "Check-Out từ xa thành công!";
                }
                else
                {
                    // Chưa có Check-In, thực hiện Check-In
                    var existingHeader = await _dbContext.ChamCongHeader1s
                        .FirstOrDefaultAsync(h => h.PsnPrkID == chamCongHeader.PsnPrkID &&
                                                  h.NgayCC == chamCongHeader.NgayCC &&
                                                  h.ThangCC == chamCongHeader.ThangCC &&
                                                  h.NamCC == chamCongHeader.NamCC);

                    ChamCongHeader1 chamCongHeader1;
                    if (existingHeader == null)
                    {
                        chamCongHeader1 = new ChamCongHeader1
                        {
                            PsnPrkID = chamCongHeader.PsnPrkID,
                            ThangCC = chamCongHeader.ThangCC,
                            NamCC = chamCongHeader.NamCC,
                            NgayCC = chamCongHeader.NgayCC
                        };
                        _dbContext.ChamCongHeader1s.Add(chamCongHeader1);
                    }
                    else
                    {
                        chamCongHeader1 = existingHeader;
                    }

                    // Tạo bản ghi Check-In
                    chamCongLine.ChamCong = 1; // Đánh dấu trạng thái Check-In
                    var newChamCongLine = new ChamCongLine1
                    {
                        Buoi = chamCongLine.Buoi,
                        ChamCong = chamCongLine.ChamCong,
                        NgayCham = chamCongLine.NgayCham,
                        NgayChinhSua = DateTime.Now,
                        GioBatDau = chamCongLine.GioBatDau ?? DateTime.Now.TimeOfDay,
                        GioKetThuc = chamCongLine.GioKetThuc ?? TimeSpan.Zero,
                        UserWritePrkID = chamCongLine.UserWritePrkID,
                        ComputerIP = chamCongLine.ComputerIP,
                        ComputerName = chamCongLine.ComputerName,
                        Status = chamCongLine.Status ?? 0,
                        StatusDescription = "Chấm công từ xa"
                    };

                    _dbContext.ChamCongLine1.Add(newChamCongLine);
                    await _dbContext.SaveChangesAsync();

                    return "Check-In từ xa thành công!";
                }
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi khi thực hiện chấm công từ xa: {ex.Message}";
            }
        }
    }
}