using AuLacChamCong.Models.Shared;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel;
using AuLacChamCong.Services.Modules.Profile.Model;
using OfficeOpenXml;

namespace AuLacChamCong.Services.TinhCong
{
    public interface ITinhCongService
    {
        IEnumerable<DeptsDb> getsKhoa();
        IEnumerable<PersonnelsDb> getsNhanVien();
        IEnumerable<PersonnelsTypeDb> getTypeNhanVien();
        IEnumerable<ChamCongTypeDb> getsLoaiChamCong();
        Task AddEmployeeAsync(PersonnelsDb newEmployee);
        Task<PersonnelsDb> GetEmployeeByIdAsync(int id);
        Task UpdateEmployeeAsync(PersonnelsDb employee);
        Task<List<DeptsDb>> GetDepartmentsAsync();
        Task<List<PersonnelsDb>> GetEmployeesAsync();
        Task<ExcelPackage> getBaoCaoNgayCongExcel(List<BaoCaoChamCong> baoCaoNgayCongModels, int daysInMonth, DateTime month);
        Task<IEnumerable<ChamCongLineModel>> GetAttendanceReportByDayAsync(DateTime? day);
        Task<IEnumerable<ChamCongLineModel>> GetAttendanceReportByMonthAsync(int? month, int? year);
        Task<IEnumerable<ChamCongLineModel>> GetAllAttendanceReportAsync();
        Task<Dictionary<int, double>> GetTotalWorkHoursPerEmployeeAsync(int? month, int? year);
        Task<List<ChamCongLineModel>> GetEmployeeDetailsAsync(int employeeId, int? month, int? year);
        Task<List<BaoCaoChamCong>> GetMonthlyAttendanceAsync(int? month, int? year, int? deptId = null, int? employeeTypeId = null);
    }

}
