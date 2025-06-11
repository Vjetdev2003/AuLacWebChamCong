namespace AuLacChamCong.Models.ViewModel
{
    public class LichLamViecViewModel
    {
        public decimal? PsnPrkID { get; set; }
        public int LoaiCongViecID { get; set; }
        public int LichLamViecID { get; set; }
        public string LoaiCongViecName { get; set; }
        public string NoiDungChiTiet { get; set; }
        public DateTime NgayTruc { get; set; }
        public TimeSpan ThoiGianBatDau { get; set; }
        public TimeSpan ThoiGianKetThuc { get; set; }
        public int PhanLoai { get; set; }
        public string DeptId { get; set; }
        public string Khoa { get; set; }
        public string PsnName { get; set; }
    }
}
