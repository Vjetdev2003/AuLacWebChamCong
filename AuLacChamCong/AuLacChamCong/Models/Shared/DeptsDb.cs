using System.ComponentModel.DataAnnotations;

namespace AuLacChamCong.Models.Shared
{
    public class DeptsDb
    {
        [Key]
        public int DeptPrkID { get; set; }
        public string? DeptPubPrkID { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public string? DeptFunc { get; set; }
        public string? DeptTypeID { get; set; }
        public string? Note { get; set; }
        public bool? IsActive { get; set; }
        public string? DeptSign { get; set; }
        public bool? IsSystem { get; set; }
        public int? DeptExmStylePrkID { get; set; }
        public bool? IsShowNoteVchDrug { get; set; }
        public string? DeptQd5084ID { get; set; }
        public byte? DeptFuncExamID { get; set; }
        public string? ListItemFeePrkID { get; set; }
        public byte? FuncExamTypeID { get; set; }
        public string? ListBedH { get; set; }
        public string? ListBedT { get; set; }
        public string? ListBedC { get; set; }
        public string? ListBedK { get; set; }
        public string? ListFeeTypePfmID { get; set; }
        public int? CountPntExamOfDeptInDay { get; set; }
        public decimal? TruongKhoaPrkID { get; set; }
        public bool? IsPfmPHCN { get; set; }
        public string? ShortName { get; set; }
    }
}
