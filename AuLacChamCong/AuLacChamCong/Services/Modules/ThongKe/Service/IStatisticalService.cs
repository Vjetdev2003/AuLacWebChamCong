using AuLacChamCong.Services.Modules.ThongKe.Model;

namespace AuLacChamCong.Services.Modules.ThongKe.Service
{
    public interface IStatisticalService
    {
        Task<List<EmployeeWorkStats>> GetTotalWorkHoursPerEmployeeAsync(int? month, int? year, int? deptId = null);
    }
}
