using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Models.Shared
{
    public class UserInfoDb
    {
        public int UserPrkID { get; set; }
        [Key]
        public string UserID { get; set; }
        public string? UserName { get; set; }
        public int UserGrpPrkID { get; set; }
        public string? UserPassword { get; set; }
        public DateTime? UserDateReg { get; set; }
        public string? UserNote { get; set; }
        public bool? IsActive { get; set; }
        public int? UserPsnPrkID { get; set; }
        public int? UserDeptWorkPrkID { get; set; }
        public string? UserInfoData { get; set; }
        public bool? IsFullRightsHSBADT { get; set; }
        public bool? IsAllowChangeChamCong { get; set; }
        public string? ListUserDeptWorkPrkID { get; set; }
        public string? KHOA_HANH_CHINH_PrkID { get; set; }
    }
}
