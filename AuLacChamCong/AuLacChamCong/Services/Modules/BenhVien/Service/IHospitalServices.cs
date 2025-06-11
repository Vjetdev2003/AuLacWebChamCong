using AuLacChamCong.Services.LocationCompany.Model;
using AuLacChamCong.Services.Modules.BenhVien.Model;

namespace AuLacChamCong.Services.Modules.BenhVien.Service
{
    public interface IHospitalServices
    {
        Task<List<HospitalDb>> GetAllHospitalsAsync();
        Task<HospitalDb> GetHospitalByIdAsync(int hospitalId);
        Task<int> CreateHospitalAsync(HospitalDb hospital);
        Task UpdateHospitalAsync(HospitalDb hospital);
        Task DeleteHospitalAsync(int hospitalId);
        Task<Coordinate> GetCoordinatesForNoiCongTac(string noiCongTac);
    }
}
