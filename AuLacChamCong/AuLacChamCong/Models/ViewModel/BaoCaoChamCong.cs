namespace AuLacChamCong.Models.ViewModel
{
    public class BaoCaoChamCong
    {
        public int UserWritePrkID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; } // Chức vụ
        public string DeptName { get; set; }
        public List<string> DailyRecords { get; set; } // Ghi chép hàng ngày (e.g., "X" for present, "" for absent)
        public int TotalWorkingDays { get; set; } // Tổng ngày làm
        public int TotalLeaveDays { get; set; } // Ngày nghỉ không lương
        public int TotalHolidays { get; set; } // Nghỉ lễ

        public BaoCaoChamCong()
        {
            DailyRecords = new List<string>();
        }
    }
}
