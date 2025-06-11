using AuLacChamCong.Models.Shared;
using AuLacChamCong.Services.Modules.Profile.Model;

namespace AuLacChamCong.Services.Modules.Profile.Service
{
    public interface IUserProfileService
    {
        Task<Personnels> GetUserProfileAsync(string userId);
    }
}
