using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Services.Modules.ChamCong.Model
{
    public class ChamCongHeaderDb
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
    }
}
