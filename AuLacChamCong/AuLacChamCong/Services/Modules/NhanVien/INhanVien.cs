using AuLacChamCong.Models.Shared;
using AuLacChamCong.Services.Modules.Profile.Model;

namespace AuLacChamCong.Services.Modules.NhanVien
{
    public interface INhanVien
    {
        Task<List<PersonnelsDb>> GetAllNhanVien();
        Task<bool> CreateNhanVien(PersonnelsDb personnelsDb); 
        Task<bool> UpdateNhanVien(PersonnelsDb personnelsDb);
    }
}
