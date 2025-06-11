using AuLacChamCong.Models.Shared.ModelSearch;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.LichNghi.Model;

namespace AuLacChamCong.Services.Modules.LichNghi.Service
{
    public interface ILichNghiServices
    {
        Task<List<LichNghiDb>> GetAllLichNghiAsync();
        Task<bool> CreateLichNghiAsync(DonXinNghiViewModel request);

        Task<bool> UpdateLichNghiStatusAsync(int lichNghiID, int status);
        Task<List<DonXinNghiViewModel>> GetLichNghiByPsnPrkID(int psnPrkID);
        Task<List<LichNghiSearch>> SearchLichNghiAsync(string? searchKeyword, int? status, string? loaiNghi);
        Task<List<LichNghiDb>> GetLichNghiHistory();
        Task<List<NghiLeDb>> GetHolidaysAsync(int? month, int? year);
        Task AddHolidayAsync(NghiLeDb nghiLeDb);
        Task UpdateHolidayAsync(NghiLeDb nghiLeDb);
        Task DeleteHolidayAsync(int holidayId);
    }
}
