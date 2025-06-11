using AuLacChamCong.Services.Modules.ChamCong.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel;

namespace AuLacChamCong.Models.ViewModel
{
    public class AttendanceReportItem
    {
        public ChamCongHeaderModel Header { get; set; }
        public List<ChamCongLineModel> Lines { get; set; }
    }
}
