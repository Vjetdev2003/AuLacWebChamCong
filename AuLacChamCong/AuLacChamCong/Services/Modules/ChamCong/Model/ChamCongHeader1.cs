using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AuLacChamCong.Models.Shared;

namespace AuLacChamCong.Services.Modules.ChamCong.Model
{
    public class ChamCongHeader1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public decimal MngChamCongPrkID { get; set; }
            [Required]
            public decimal PsnPrkID { get; set; }
            [Required]
            public int ThangCC { get; set; }
            [Required]
            public int NamCC { get; set; }
            [Required]
            public int NgayCC { get; set; }

        // Navigation property cho ChamCongLine1
        public virtual ICollection<ChamCongLine1> ChamCongLines { get; set; } = new List<ChamCongLine1>();    }
}
