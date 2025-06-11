using AuLacChamCong.Models.Shared.ModelSearch;
using AuLacChamCong.Models.ViewModel;
using AuLacChamCong.Services.Modules.DonCongTac.Model;

namespace AuLacChamCong.Services.Modules.DonCongTac.Service
{
    public interface IDonCongTacServices
    {
        Task<bool> CreateDonCongTacAsync(DonCongTacViewModel donCongTac);
        Task<List<DonCongTacDb>> GetAllDonCongTacAsync();
        Task<bool> UpdateDonCongTacStatusAsync(int mngDonCongTacPrkID, int status);
        Task<List<DonCongTacDb>> GetHistoryDonCongTac();   
        Task<List<DonCongTacViewModel>> GetHistoryDonCongTacByPsnPrkIDAsync(int psnPrkID);

        Task<List<DonCongTacSearch>>SearchDonCongTacAsync(string searchKeyword, int? status, string? noiCongTac);

        Task<bool>SaveDonCongTacAsync(DonCongTacViewModel donCongTac);
    }
}
