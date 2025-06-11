using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Models.Shared
{
    public class ChamCongTypeDb
    {
        [Key]
        public int IdTypeChamCong { get; set; }
        public string? KyHieuChamCong { get; set; }
        public string? TenLoaiChamCong { get; set; }
    }
}
