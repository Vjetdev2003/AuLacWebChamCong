using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Services.Modules.LichLamViec.Model
{
    public class LichLamViecDb
    {
        [Key]
        public int LichLamViecID { get; set; }
        public decimal? PsnPrkID { get; set; }
        public int LoaiCongViecID { get; set; }
        public string NoiDungChiTiet { get; set; }
        public DateTime NgayTruc { get; set; }
        public TimeSpan ThoiGianBatDau { get; set; }
        public TimeSpan ThoiGianKetThuc { get; set; }
        public int PhanLoai { get; set; }
    }
}
