using AuLacChamCong.Services.Modules.ChamCong.Model.FaceId;

namespace AuLacChamCong.Services.Modules.ChamCong.Services.FaceIdSerivces
{
    public interface IFaceIdService
    {
        Task<bool> DetectFace(byte[] faceImage);
        Task<string> RegisterFace(decimal psnPrkID, List<string> encodedImages);
        Task<bool> VerifyFace(decimal psnPrkID, byte[] faceImage);
        Task<List<FaceId>> GetAllFaceId();
    }
}
