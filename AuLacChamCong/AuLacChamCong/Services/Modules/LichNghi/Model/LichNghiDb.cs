using AuLacChamCong.Models.Shared;
using System;

namespace AuLacChamCong.Services.Modules.LichNghi.Model
{
    public class LichNghiDb
    {
        public int? LichNghiID { get; set; }
        public int? PsnPrkID { get; set; }
        public int? LoaiNghiID { get; set; }
        public string? LoaiNghiName { get; set; }
        public DateTime NgayNopDon { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string LyDo { get; set; }
        public int? Status { get; set; }
        public string? StatusDescription { get; set; }
       // public string? CaLamViec { get; set; }

        public PersonnelsDb Personnels { get; set; }
    }
}
