using AuLacChamCong.Models.Shared;

namespace AuLacChamCong.Services.Modules.DonCongTac.Model
{
    public class DonCongTacDb
    {
        public int MngDonCongTacPrkID { get; set; }
        public int PsnPrkID { get; set; }
        public DateTime NgayCongTac { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string NoiCongTac { get; set; }
        public string LyDo { get; set; }
        public int? Status { get; set; }
        public string? StatusDescription { get; set; }
        public int? HospitalId { get; set; }
        //public string StatusDescription { 
        //    get {
        //        switch (Status)
        //        {
        //            case Constants.Accepted:
        //                return "Đã chấp nhận";
        //            case Constants.Rejected:
        //                return "Đã từ chối";
        //            case Constants.Pending:
        //                return "Chờ xác nhận";
        //            default:
        //                return "Không xác định";
        //        }
        //     }
        //}
        public PersonnelsDb Personnels { get; set; }
    }
}