using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuLacChamCong.Services.Modules.ChamCong.Model
{
    public class ChamCongLine1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal? MngChamCongPrkID { get; set; }
        [Key]
        public int? Buoi { get; set; }
        public int? ChamCong { get; set; }
        public DateTime NgayCham { get; set; }
        public DateTime? NgayChinhSua { get; set; }
        public TimeSpan? GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public decimal? UserWritePrkID { get; set; }
        public string? ComputerIP { get; set; }
        public string? ComputerName { get; set; }
        public int? Status { get; set; }
        public string? StatusDescription { get; set; }
        // Navigation property cho ChamCongHeader1
        [ForeignKey("MngChamCongPrkID")]
        public virtual ChamCongHeader1 ChamCongHeader { get; set; }

    }
}
