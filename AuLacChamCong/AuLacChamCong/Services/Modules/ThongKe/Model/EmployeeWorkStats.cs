namespace AuLacChamCong.Services.Modules.ThongKe.Model
{
    public class EmployeeWorkStats
    {
        public decimal PsnPrkID { get; set; }
        public string PsnName { get; set; }
        public int AttendedDays { get; set; } // Tổng ngày công
        public double TotalHours { get; set; } // Tổng giờ làm (tính bằng giờ, dạng double)
    }
}
