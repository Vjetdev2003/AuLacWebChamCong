namespace AuLacChamCong.Models.Shared.ModelSearch
{
    public class LichNghiSearch
    {
        public int? LichNghiID { get; set; }
        public int? PsnPrkID { get; set; }
        public int? LoaiNghiID { get; set; }
        public string? LoaiNghiName { get; set; }
        public DateTime NgayNopDon { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string LyDo { get; set; }
        public int? Status { get; set; }
        public string? StatusDescription { get; set; }
        // public string? CaLamViec { get; set; }

        public string PersonnelsName { get; set; }
    }
}
