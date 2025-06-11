namespace AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel
{
   public class ChamCongLineDetail
    {
        public decimal? MngChamCongPrkID { get; set; }
        public int? Buoi { get; set; }
        public int? ChamCong { get; set; }
        public DateTime NgayCham { get; set; }
        public DateTime? NgayChinhSua { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; } // Thêm ? vì GioKetThuc trong ChamCongLine1 có thể NULL
        public int? UserWritePrkID { get; set; }
        public string? ComputerIP { get; set; }
        public string? ComputerName { get; set; }
        public bool OnTimeStatus { get; set; } // Thêm thuộc tính này
        public int? Status { get; set; }
    }
}
