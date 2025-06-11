using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel
{
    public class ChamCongHeaderModel
    {
        public decimal MngChamCongPrkID { get; set; }
        public decimal PsnPrkID { get; set; }
        public int ThangCC { get; set; }
        public int NamCC { get; set; }
        public int NgayCC { get; set; }
    }
}
