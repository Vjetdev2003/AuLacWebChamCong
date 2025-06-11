using AuLacChamCong.DataApi;
using AuLacChamCong.Services.Modules.ChamCong.Services;
using AuLacChamCong.Services.Modules.ThongKe.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AuLacChamCong.Services.Modules.ThongKe.Service
{
    public class StatisticalService : IStatisticalService
    {
        private readonly ApplicationDbContext _dbContext;

        public StatisticalService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EmployeeWorkStats>> GetTotalWorkHoursPerEmployeeAsync(int? month, int? year, int? deptId = null)
        {
            try
            {
                // Nếu không có tháng/năm, lấy tháng/năm hiện tại
                month ??= DateTime.Now.Month;
                year ??= DateTime.Now.Year;

                var employeesQuery = _dbContext.Personnels.AsQueryable();
                if (deptId.HasValue)
                {
                    string deptIdString = $",{deptId.Value},";
                    employeesQuery = employeesQuery.Where(e => ("," + e.DeptPrkID + ",").Contains(deptIdString));
                }
                var employees = await employeesQuery
                    .Select(p => new { p.PsnPrkID, p.PsnName })
                    .ToListAsync();

                // Lấy danh sách ngày lễ trong tháng/năm
                var holidays = await _dbContext.NghiLeDbs
                    .Where(h => h.HolidayDate.Month == month && h.HolidayDate.Year == year)
                    .Select(h => h.HolidayDate.Date)
                    .Where(date => date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday)
                    .ToListAsync();

                // Lấy dữ liệu chấm công trong tháng/năm
                var attendanceRecords = await _dbContext.ChamCongLine1
                    .Where(record => record.NgayCham.Month == month.Value && record.NgayCham.Year == year.Value && record.UserWritePrkID.HasValue)
                    .ToListAsync();

                var result = new List<EmployeeWorkStats>();

                // Tính toán cho từng nhân viên
                foreach (var employee in employees)
                {
                    var employeeRecords = attendanceRecords
                        .Where(record => record.UserWritePrkID.HasValue && (int)record.UserWritePrkID.Value == (int)employee.PsnPrkID)
                        .ToList();

                    var stats = new EmployeeWorkStats
                    {
                        PsnPrkID = employee.PsnPrkID,
                        PsnName = employee.PsnName,
                        AttendedDays = 0,
                        TotalHours = 0
                    };

                    // Tính tổng ngày công
                    var attendedDates = employeeRecords
                        .GroupBy(r => r.NgayCham.Date)
                        .Select(g => g.Key)
                        .ToList();

                    attendedDates = attendedDates.Union(holidays).Distinct().ToList();
                    stats.AttendedDays = attendedDates.Count;

                    // Tính tổng giờ làm
                    double totalHours = 0;
                    foreach (var record in employeeRecords)
                    {
                        if (record.GioBatDau.HasValue && record.GioKetThuc.HasValue && record.GioBatDau != TimeSpan.Zero && record.GioKetThuc != TimeSpan.Zero)
                        {
                            var duration = record.GioKetThuc.Value - record.GioBatDau.Value;
                            if (duration.TotalHours >= 0)
                            {
                                totalHours += duration.TotalHours;
                            }
                        }
                    }

                    // Cộng thêm 8 giờ cho mỗi ngày lễ từ thứ Hai đến thứ Sáu
                    foreach (var holiday in holidays)
                    {
                        if (!employeeRecords.Any(r => r.NgayCham.Date == holiday))
                        {
                            totalHours += 8; // Cộng 8 giờ
                        }
                    }

                    stats.TotalHours = Math.Round(totalHours, 2); // Làm tròn 2 chữ số thập phân
                    result.Add(stats);
                }

                return result.OrderBy(r => r.PsnName).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi tính tổng giờ làm và ngày công: {ex.Message}");
                return new List<EmployeeWorkStats>();
            }
        }
    }
}
