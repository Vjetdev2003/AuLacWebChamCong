using AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel;

namespace AuLacChamCong.Services.Modules.ChamCong.Model
{
    public class HistoryResponse
    {
        public decimal? MngChamCongPrkID { get; set; }
        public decimal PsnPrkID { get; set; }
        public int? ThangCC { get; set; }
        public int? NamCC { get; set; }
        public int? NgayCC { get; set; }
        public List<ChamCongLineModel> Lines { get; set; } = new List<ChamCongLineModel>();
    }
}
