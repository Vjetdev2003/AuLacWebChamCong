using AuLacChamCong.DataApi;
using AuLacChamCong.Models.Shared;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.ChamCong.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel;
using AuLacChamCong.Services.Modules.Profile.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Net.Http;

namespace AuLacChamCong.Services.TinhCong
{
    public class TinhCongServices : ITinhCongService
    {
        private readonly ApplicationDbContext _tinhCongDbContext;
        private readonly HttpClient _httpClient;
        private static List<DeptsDb> _cachedDepartments = null;
        public TinhCongServices(ApplicationDbContext tinhCongDbContext, HttpClient httpClient)
        {
            _tinhCongDbContext = tinhCongDbContext;
            _httpClient = httpClient;
        }

        public async Task AddEmployeeAsync(PersonnelsDb newEmployee)
        {
            await _tinhCongDbContext.Personnels.AddAsync(newEmployee);
            await _tinhCongDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChamCongLineModel>> GetAllAttendanceReportAsync()
        {
            return await _tinhCongDbContext.ChamCongLine1
                      .Select(c => new ChamCongLineModel
                      {
                          MngChamCongPrkID = c.MngChamCongPrkID,
                          Buoi = c.Buoi,
                          ChamCong = c.ChamCong,
                          NgayCham = c.NgayCham,
                          NgayChinhSua = c.NgayChinhSua,
                          GioBatDau = c.GioBatDau,
                          GioKetThuc = c.GioKetThuc,
                          UserWritePrkID = c.UserWritePrkID,
                          ComputerIP = c.ComputerIP,
                          ComputerName = c.ComputerName,
                          Status = c.Status
                      }).ToListAsync();
        }

        public async Task<IEnumerable<ChamCongLineModel>> GetAttendanceReportByDayAsync(DateTime? day)
        {
            if (!day.HasValue)
            {
                day = DateTime.Now; // Nếu không chọn ngày, lấy ngày hiện tại
            }

            return await _tinhCongDbContext.ChamCongLine1
                .Where(c => c.NgayCham.Date == day.Value.Date)
                .Select(c => new ChamCongLineModel
                {
                    MngChamCongPrkID = c.MngChamCongPrkID,
                    Buoi = c.Buoi,
                    ChamCong = c.ChamCong,
                    NgayCham = c.NgayCham,
                    NgayChinhSua = c.NgayChinhSua,
                    GioBatDau = c.GioBatDau,
                    GioKetThuc = c.GioKetThuc,
                    UserWritePrkID = c.UserWritePrkID,
                    ComputerIP = c.ComputerIP,
                    ComputerName = c.ComputerName,
                    Status = c.Status
                }).ToListAsync();
        }

        public async Task<IEnumerable<ChamCongLineModel>> GetAttendanceReportByMonthAsync(int? month, int? year)
        {
            month ??= DateTime.Now.Month; // Nếu không chọn tháng, lấy tháng hiện tại
            year ??= DateTime.Now.Year; // Nếu không chọn năm, lấy năm hiện tại

            return await _tinhCongDbContext.ChamCongLine1
                .Where(c => c.NgayCham.Month == month && c.NgayCham.Year == year)
                .Select(c => new ChamCongLineModel
                {
                    MngChamCongPrkID = c.MngChamCongPrkID,
                    Buoi = c.Buoi,
                    ChamCong = c.ChamCong,
                    NgayCham = c.NgayCham,
                    NgayChinhSua = c.NgayChinhSua,
                    GioBatDau = c.GioBatDau,
                    GioKetThuc = c.GioKetThuc,
                    UserWritePrkID = c.UserWritePrkID,
                    ComputerIP = c.ComputerIP,
                    ComputerName = c.ComputerName,
                    Status = c.Status
                }).ToListAsync();
        }

        public async Task<ChamCongLineModel> GetEmployeeAttendanceByDayAsync(int employeeId, DateTime? day)
        {
            if (!day.HasValue)
            {
                day = DateTime.Now; // Nếu không chọn ngày, lấy ngày hiện tại
            }

            return await _tinhCongDbContext.ChamCongLine1
                .Where(c => c.UserWritePrkID == employeeId && c.NgayCham.Date == day.Value.Date)
                .Select(c => new ChamCongLineModel
                {
                    MngChamCongPrkID = c.MngChamCongPrkID,
                    Buoi = c.Buoi,
                    ChamCong = c.ChamCong,
                    NgayCham = c.NgayCham,
                    NgayChinhSua = c.NgayChinhSua,
                    GioBatDau = c.GioBatDau,
                    GioKetThuc = c.GioKetThuc,
                    UserWritePrkID = c.UserWritePrkID,
                    ComputerIP = c.ComputerIP,
                    ComputerName = c.ComputerName,
                    Status = c.Status
                }).FirstOrDefaultAsync();
        }

        public async Task<PersonnelsDb> GetEmployeeByIdAsync(int id)
        {
            var response = await _tinhCongDbContext.Personnels.SingleOrDefaultAsync(t=>t.PsnPrkID == id);

            return response;
        }
        public IEnumerable<DeptsDb> getsKhoa()
        {
            return _tinhCongDbContext.Depts.ToList();
        }

        public IEnumerable<ChamCongTypeDb> getsLoaiChamCong()
        {
            return _tinhCongDbContext.ChamCongTypes.ToList();
        }

        public  IEnumerable<PersonnelsDb> getsNhanVien()
        {
            return _tinhCongDbContext.Personnels.ToList();
        }
        public async Task<List<ChamCongLineModel>> GetEmployeeDetailsAsync(int employeeId, int? month, int? year)
        {
            return await _tinhCongDbContext.ChamCongLine1
                .Where(c => c.UserWritePrkID == employeeId &&
                            (month == null || c.NgayCham.Month == month) &&
                            (year == null || c.NgayCham.Year == year))
                .Select(c => new ChamCongLineModel
                {
                    MngChamCongPrkID = c.MngChamCongPrkID,
                    Buoi = c.Buoi,
                    ChamCong = c.ChamCong,
                    NgayCham = c.NgayCham,
                    NgayChinhSua = c.NgayChinhSua,
                    GioBatDau = c.GioBatDau,
                    GioKetThuc = c.GioKetThuc,
                    UserWritePrkID = c.UserWritePrkID,
                    ComputerIP = c.ComputerIP,
                    ComputerName = c.ComputerName,
                    Status = c.Status
                }).ToListAsync();
        }

        public IEnumerable<PersonnelsTypeDb> getTypeNhanVien()
        {
            return _tinhCongDbContext.PersonnelsTypes.ToList();
        }

        public async Task UpdateEmployeeAsync(PersonnelsDb employee)
        {
             _tinhCongDbContext.Personnels.Update(employee);
            await _tinhCongDbContext.SaveChangesAsync();
        }

        public async Task<Dictionary<int, double>> GetTotalWorkHoursPerEmployeeAsync(int? month, int? year)
        {
            try
            {
                month ??= DateTime.Now.Month;
                year ??= DateTime.Now.Year;

                var query = _tinhCongDbContext.ChamCongLine1
                    .Where(record =>
                        record.NgayCham.Month == month.Value &&
                        record.NgayCham.Year == year.Value &&
                        record.UserWritePrkID.HasValue);

                var totalHoursByEmployee = await query
                    .GroupBy(record => record.UserWritePrkID)
                    .Select(group => new
                    {
                        PsnPrkID = group.Key,
                        TotalMinutes = group
                            .Where(record => record.GioBatDau.HasValue && record.GioKetThuc.HasValue)
                            .Sum(record => EF.Functions.DateDiffMinute(record.GioBatDau.Value, record.GioKetThuc.Value))
                    })
                    .ToDictionaryAsync(
                        x => (int)x.PsnPrkID!.Value,
                        x => x.TotalMinutes / 60.0 // Chuyển đổi phút thành giờ
                    );

                return totalHoursByEmployee;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi tính tổng giờ làm: {ex.Message}");
                return new Dictionary<int, double>();
            }
        }

        public async Task<List<BaoCaoChamCong>> GetMonthlyAttendanceAsync(int? month, int? year, int? deptId = null, int? employeeId = null)
        {
            try
            {
                month ??= DateTime.Now.Month;
                year ??= DateTime.Now.Year;

                int daysInMonth = DateTime.DaysInMonth(year.Value, month.Value);
                Console.WriteLine($"[INFO] GetMonthlyAttendanceAsync: Tạo báo cáo cho tháng {month}/{year}, deptId: {deptId}, employeeId: {employeeId}");

                var employeesQuery = _tinhCongDbContext.Personnels.AsQueryable();

                if (deptId.HasValue)
                {
                    // Format the deptId as a string and ensure it's surrounded by commas in the search
                    string deptIdString = $",{deptId.Value},";
                    employeesQuery = employeesQuery.Where(e => ("," + e.DeptPrkID + ",").Contains(deptIdString));
                    Console.WriteLine("[INFO] GetMonthlyAttendanceAsync: Đã áp dụng bộ lọc deptId.");
                }

                if (employeeId.HasValue)
                {
                    employeesQuery = employeesQuery.Where(e => e.PsnPrkID == employeeId.Value);
                    Console.WriteLine($"[INFO] GetMonthlyAttendanceAsync: Đã áp dụng bộ lọc employeeId: {employeeId}");
                }

                var employees = await employeesQuery.ToListAsync();
                Console.WriteLine($"[INFO] GetMonthlyAttendanceAsync: Đã lấy được {employees.Count} nhân viên từ database.");

                var attendanceRecords = await _tinhCongDbContext.ChamCongLine1
                    .Where(record =>
                        record.NgayCham.Month == month.Value &&
                        record.NgayCham.Year == year.Value &&
                        record.UserWritePrkID.HasValue)
                    .ToListAsync();
                Console.WriteLine($"[INFO] GetMonthlyAttendanceAsync: Đã lấy được {attendanceRecords.Count} bản ghi chấm công.");

                var report = new List<BaoCaoChamCong>();

                foreach (var employee in employees)
                {
                    var employeeRecords = attendanceRecords
                        .Where(record => (int)record.UserWritePrkID!.Value == employee.PsnPrkID)
                        .ToList();

                    var baoCao = new BaoCaoChamCong
                    {
                        UserWritePrkID = employee.PsnPrkID,
                        EmployeeName = employee.PsnName,
                        Position = employee.Chucvu,
                        DailyRecords = new List<string>()
                    };

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        baoCao.DailyRecords.Add("0");
                    }

                    int totalWorkingDays = 0;
                    int totalLeaveDays = 0;
                    int totalHolidays = 0;

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var date = new DateTime(year.Value, month.Value, day);
                        var record = employeeRecords.FirstOrDefault(r => r.NgayCham.Date == date);

                        if (record == null)
                        {
                            totalLeaveDays++;
                        }
                        else
                        {
                            switch (record.Status)
                            {
                                case 0:
                                    baoCao.DailyRecords[day - 1] = "0";
                                    totalLeaveDays++;
                                    break;
                                case 1:
                                    baoCao.DailyRecords[day - 1] = "X";
                                    totalWorkingDays++;
                                    break;
                                case 2:
                                    baoCao.DailyRecords[day - 1] = "Muộn";
                                    totalWorkingDays++;
                                    break;
                                default:
                                    baoCao.DailyRecords[day - 1] = "0";
                                    break;
                            }
                        }

                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if (baoCao.DailyRecords[day - 1] == "0")
                            {
                                baoCao.DailyRecords[day - 1] = "L";
                                totalLeaveDays--;
                                totalHolidays++;
                            }
                        }
                    }

                    baoCao.TotalWorkingDays = totalWorkingDays;
                    baoCao.TotalLeaveDays = totalLeaveDays;
                    baoCao.TotalHolidays = totalHolidays;

                    report.Add(baoCao);
                }

                Console.WriteLine($"[INFO] GetMonthlyAttendanceAsync: Báo cáo hoàn tất với {report.Count} bản ghi.");
                return report.OrderBy(r => r.EmployeeName).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi tạo báo cáo chấm công: {ex.Message}");
                return new List<BaoCaoChamCong>();
            }
        }
        public async Task<List<DeptsDb>> GetDepartmentsAsync()
        {
            try
            {
                // Return cached departments if available
                if (_cachedDepartments != null)
                {
                    return _cachedDepartments;
                }

                // Fetch departments from the database
                var departments =  _tinhCongDbContext.Depts.ToList();

                // Cache the result
                _cachedDepartments = departments;

                return departments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lấy danh sách phòng ban: {ex.Message}");
                return new List<DeptsDb>();
            }
        }
        public async Task<List<PersonnelsDb>> GetEmployeesAsync()
        {
            try
            {
                var employees = await _tinhCongDbContext.Personnels
                    .OrderBy(e => e.PsnName)
                    .ToListAsync();

                return employees;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lấy danh sách nhân viên: {ex.Message}");
                return new List<PersonnelsDb>();
            }
        }

        public async Task<ExcelPackage> getBaoCaoNgayCongExcel(List<BaoCaoChamCong> baoCaoNgayCongModels, int daysInMonth, DateTime month)
        {
            List<string> thuNgay = new List<string>() { "CN", "Th2", "Th3", "Th4", "Th5", "Th6", "Th7" };
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var excel = new ExcelPackage();

            var workSheet = excel.Workbook.Worksheets.Add(month.ToString("MM/yyyy"));
            workSheet.Cells[1, 1].RichText.Add("Khoa").Bold = true;
            workSheet.Cells["A1:A2"].Merge = true;
            workSheet.Cells["A1:A2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            workSheet.Cells["A1:A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            workSheet.Column(1).Width = 18;

            workSheet.Cells[1, 2].RichText.Add("Họ và tên").Bold = true;
            workSheet.Cells["B1:B2"].Merge = true;
            workSheet.Cells["B1:B2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            workSheet.Cells["B1:B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            workSheet.Column(2).Width = 32;

            workSheet.Cells[1, daysInMonth + 3].RichText.Add("Tổng ngày công").Bold = true;
            workSheet.Cells[1, daysInMonth + 3, 2, daysInMonth + 3].Merge = true;
            workSheet.Cells[1, daysInMonth + 3, 2, daysInMonth + 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, daysInMonth + 3, 2, daysInMonth + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            workSheet.Column(daysInMonth + 3).Width = 15;

            // Set day-of-week labels
            for (int i = 1; i <= daysInMonth; i++)
            {
                workSheet.Cells[1, i + 2].RichText.Add(i.ToString()).Bold = true;
                workSheet.Cells[1, i + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                int b = (int)(new DateTime(month.Year, month.Month, i)).DayOfWeek;
                workSheet.Cells[2, i + 2].RichText.Add(thuNgay[b]).Bold = true;
                workSheet.Cells[2, i + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                workSheet.Column(i + 2).Width = 4;
            }

            // Set values
            for (int j = 1; j <= baoCaoNgayCongModels.Count; j++)
            {
                // Set department and employee name
                workSheet.Cells[j + 2, 1].Value = string.IsNullOrEmpty(baoCaoNgayCongModels[j - 1].DeptName) ? "N/A" : baoCaoNgayCongModels[j - 1].DeptName;
                workSheet.Cells[j + 2, 2].Value = baoCaoNgayCongModels[j - 1].EmployeeName;

                // Set attendance records
                double count = 0;
                for (int i = 1; i <= daysInMonth; i++)
                {
                    var chamCongValue = baoCaoNgayCongModels[j - 1].DailyRecords[i - 1];
                    if (chamCongValue != "0" && double.TryParse(chamCongValue, out double value))
                    {
                        count += value;
                    }
                    workSheet.Cells[j + 2, i + 2].Value = chamCongValue;
                }
                workSheet.Cells[j + 2, daysInMonth + 3].Value = count;
            }

            return excel;
        }
    }
}
