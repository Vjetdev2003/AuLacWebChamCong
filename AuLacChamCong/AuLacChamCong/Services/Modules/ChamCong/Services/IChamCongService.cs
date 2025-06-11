using AuLacChamCong.Services.Modules.ChamCong.Model;
using AuLacChamCong.Services.Modules.ChamCong.Model.ViewModel;

namespace AuLacChamCong.Services.Modules.ChamCong.Services
{
    public interface IChamCongService
    {
        decimal CheckChamCong(ChamCongHeader1 chamCongHeader);
        string createChamCong(ChamCongLine1 chamCongLine);
        string updateChamCong(ChamCongLine1 chamCongLine);
        string deleteChamCong(ChamCongLine1 chamCongLine);
        Task<List<HistoryResponse>> GetChamCongHistory(decimal psnPrkID, int? month = null, int? year = null);
        Task<string> CheckInAsync(ChamCongLineModel chamCongLine,ChamCongHeaderModel chamCongHeader);
        Task<string> CheckOutAsync(ChamCongLineModel chamCongLine);

        Task<string> ChamCongTuXa(ChamCongLineModel chamCongLine, ChamCongHeaderModel chamCongHeader);
    }
}
