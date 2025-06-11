using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel
{
    public class ChamCongDetailModel
    {
        public ChamCongHeaderModel ChamCongHeader { get; set; }
        public ChamCongLineModel ChamCongLines { get; set; }
    }
}
