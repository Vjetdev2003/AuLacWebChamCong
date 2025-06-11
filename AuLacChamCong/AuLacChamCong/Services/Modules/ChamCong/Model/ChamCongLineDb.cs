using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Services.Modules.ChamCong.Model
{
    public class ChamCongLineDb
    {
        [Key]
        public decimal? MngChamCongPrkID { get; set; }
        [Key]
        public int? Buoi { get; set; }
        public int? ChamCong { get; set; }
        public DateTime NgayCham { get; set; }
        public DateTime? NgayChinhSua { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
    }
}
