using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.LichLamViec.Model;

namespace AuLacChamCong.Services.Modules.LichLamViec.Service
{
    public interface ILichLamViecServices 
    {
        Task<List<LichLamViecDb>> GetAllLichLamViecsAsync();
        Task<string>CreateLichLamViecAsync(LichLamViecDb lichLamViec);
        Task<string>UpdateLichLamViecAsync(LichLamViecDb lichLamViec);
        Task<bool>DeleteLichLamViecAsync(int lichLamViecId);
        Task<List<LichLamViecViewModel>?> Search(LichLamViecSearch LichLamViec);
        Task<LichLamViecDb> GetLichLamViecByIdAsync(int lichLamViecId);
    }
}
